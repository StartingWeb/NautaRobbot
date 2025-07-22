using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TransactSQL
{

    ///Lists
    public List<object> list_campos = new List<object>();
    public List<object> list_where = new List<object>();


    ///Ações
    private bool acao_insert = false;
    private bool acao_update = false;
    private bool acao_delete = false;


    ///Instruções
    public string str_instrucao_sql = "";
    private string str_instrucao_where = "";

    public readonly SQL _conexao;


    public TransactSQL()
    {
        _conexao = new SQL();
        str_instrucao_sql = "";
        str_instrucao_where = "";

        list_campos = new List<object>();
        list_where = new List<object>();
    }

    public void add(string campo, string valor, bool ehTexto = true)
    {
        list_campos.Add(new
        {
            campo = campo,
            valor = valor,
            ehTexto = ehTexto
        });
    }


    public void where(string campo, string valor, bool ehTexto = true)
    {
        list_where.Add(new
        {
            campo = campo,
            valor = valor,
            ehTexto = ehTexto
        });
    }

    private void construirWhere()
    {
        StringBuilder sb_where = new StringBuilder();


        if (list_where.Count > 0)
        {
            sb_where.AppendLine(" WHERE 1=1");

            foreach (dynamic obj in list_where)
            {
                string campo = "" + obj.campo;
                string valor = "" + obj.valor;
                bool ehTexto = obj.ehTexto;

                if (ehTexto)
                    sb_where.AppendLine(" and " + campo + " = '" + valor + "'");
                else
                    sb_where.AppendLine(" and " + campo + " = " + valor);

            }

            str_instrucao_where = sb_where.ToString();
        }
        else
        {
            //Sem ação para esse caso
        }
    }


    //C.U.D.
    public void insert(string tabela, bool bl_ultimo_registro = true)
    {
        acao_insert = true;

        StringBuilder sb_insert = new StringBuilder();
        sb_insert.AppendLine("INSERT INTO ");
        sb_insert.AppendLine(tabela);
        sb_insert.AppendLine(" (");

        //Loop para montar os campos
        foreach (dynamic obj in list_campos)
        {
            string campo = "" + obj.campo;
            string separador = obj == list_campos.Last() ? "" : ","; //Se for o ultimo objeto do loop ele não adiciona a virgula
            sb_insert.AppendLine(campo + separador);
        }

        sb_insert.AppendLine(") ");
        sb_insert.AppendLine("Values ");
        sb_insert.AppendLine(" (");

        //Loop para montar os campos
        foreach (dynamic obj in list_campos)
        {
            string campo = "" + obj.campo;
            string valor = "" + obj.valor;
            bool ehTexto = obj.ehTexto;
            string separador = obj == list_campos.Last() ? "" : ","; //Se for o ultimo objeto do loop ele não adiciona a virgula

            if (campo.StartsWith("chr_") && ehTexto)
            {
                _conexao.prm("@" + campo, valor);
                valor = "@" + campo + "";
            }
            else if (campo.StartsWith("dt_"))
            {
                if (valor.ToLower().Contains("getdate"))
                {
                    valor = "getdate()";
                }
                else
                {
                    if (valor.Contains("-"))
                    {
                        string[] partes = valor.Split('-');

                        if (partes.Length == 3 && partes[0].Length == 4) // Verifica se o primeiro valor tem 4 caracteres (ano)
                        {
                            // Reordena as partes: AAAA-MM-DD -> DD-MM-AAAA
                            valor = partes[2] + "-" + partes[1] + "-" + partes[0];
                        }
                    }

                    _conexao.prm("@" + campo, valor);
                    if (ehTexto) valor = "convert(Date, @" + campo + ", 103)";
                }

            }
            else if (campo.StartsWith("fl_"))
            {
                _conexao.prm("@" + campo, valor);
                valor = "'" + valor + "'";
            }
            else if (campo.StartsWith("flo_"))
            {
                valor = valor.Trim();
                valor = valor.Replace("R$", "");
                if (!valor.ToLower().Contains("isnull"))
                    valor = valor.Replace(".", "").Replace(",", ".");
            }



            sb_insert.AppendLine(valor + separador);
        }
        sb_insert.AppendLine("); ");

        if (bl_ultimo_registro)
            sb_insert.AppendLine(" SELECT SCOPE_IDENTITY(); ");



        str_instrucao_sql = sb_insert.ToString();
    }


    public void update(string tabela)
    {
        acao_update = true;

        StringBuilder sb_update = new StringBuilder();
        sb_update.AppendLine("UPDATE ");
        sb_update.AppendLine(tabela);
        sb_update.AppendLine(" SET ");

        // Loop para montar os campos
        foreach (dynamic obj in list_campos)
        {
            string campo = "" + obj.campo;
            string valor = "" + obj.valor;
            bool ehTexto = obj.ehTexto;
            string separador = obj == list_campos.Last() ? "" : ","; // Se for o último objeto do loop ele não adiciona a vírgula

            if (campo.StartsWith("chr_") && ehTexto)
            {
                _conexao.prm("@" + campo, valor);
                valor = "@" + campo + "";
            }
            else if (campo.StartsWith("dt_"))
            {
                if (valor.ToLower().Contains("getdate"))
                {
                    valor = "getdate()";
                }
                else
                {
                    if (valor.Contains("-"))
                    {
                        string[] partes = valor.Split('-');

                        if (partes.Length == 3 && partes[0].Length == 4) // Verifica se o primeiro valor tem 4 caracteres (ano)
                        {
                            // Reordena as partes: AAAA-MM-DD -> DD-MM-AAAA
                            valor = partes[2] + "-" + partes[1] + "-" + partes[0];
                        }
                    }

                    _conexao.prm("@" + campo, valor);
                    if (ehTexto) valor = "convert(Date, @" + campo + ", 103)";
                }
            }
            else if (campo.StartsWith("fl_"))
            {
                _conexao.prm("@" + campo, valor);
                valor = "'" + valor + "'";
            }
            else if (campo.StartsWith("flo_"))
            {
                valor = valor.Trim();
                valor = valor.Replace("R$", "");
                if (!valor.ToLower().Contains("isnull"))
                    valor = valor.Replace(".", "").Replace(",", ".");
            }


            sb_update.AppendLine(campo + " = " + valor + separador);
        }

        // Adiciona a cláusula WHERE
        construirWhere();

        if (!str_instrucao_where.Equals(""))
        {
            sb_update.AppendLine(str_instrucao_where);
        }
        else
        {
            // Sem cláusula WHERE, atualiza todos os registros
        }

        str_instrucao_sql = sb_update.ToString();
    }


    public void delete(string tabela)
    {
        acao_delete = true;
        construirWhere();

        StringBuilder sb_delete = new StringBuilder();
        sb_delete.AppendLine("DELETE FROM ");
        sb_delete.AppendLine(tabela);
        if (!str_instrucao_where.Equals(""))
            sb_delete.AppendLine(str_instrucao_where);

        str_instrucao_sql = sb_delete.ToString();
    }


    public string exec()
    {
        string retorno = null;
        if (!str_instrucao_sql.Equals(""))
        {
            ///Debug
            //HttpContext.Current.Response.Write(str_instrucao_sql);
            //HttpContext.Current.Response.End();

            SqlDataReader sdr = _conexao.DataReader(str_instrucao_sql);
            if (sdr.Read())
            {
                if (acao_insert)
                {
                    //Retorna o ID que acabou de ser inserido!
                    retorno = "" + sdr[0];
                }
            }
        }
        else
        {
            //Sem ação para esse caso de uso!
        }

        _conexao.Close();


        return retorno;
    }

}