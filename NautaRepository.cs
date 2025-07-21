using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
public class NautaRepository
{
    public readonly SQL _conexao;
    public List<CampoSQL> camposSQL = new List<CampoSQL>();

    public class CampoSQL
    {
        public string chave;
        public string chaveExtra; //Para campos do tipo DropDownList
        public string valor;
        public bool valorMonetario = false;
        public bool ehTexto = true;
        public Type tipoComponente;
    }

    public NautaRepository()
    {
        _conexao = new SQL();
    }


    public DataTable RetornaDadosPesquisa(NautaModelSQL nautaSQL)
    {
        StringBuilder camposPesquisa = new StringBuilder();
        StringBuilder wherePesquisa = new StringBuilder();

        foreach (CampoSQL campo in camposSQL)
        {

            if (!campo.valor.Equals(""))
            {
                if (campo.tipoComponente == typeof(CompTextBox))
                {
                    _conexao.prm("@" + campo.chave, campo.valor);
                    wherePesquisa.AppendLine(" AND " + campo.chave + " like '%'+ @" + campo.chave + " +'%'");
                }
                else if (campo.tipoComponente == typeof(CompCalendario))
                {
                    wherePesquisa.AppendLine(" AND (1 = 1 ");

                    var partes = campo.valor.Split(',');
                    string dataInicial = ("" + partes[0]).ToString().Trim();
                    if (!dataInicial.Equals(""))
                    {
                        _conexao.prm("@" + campo.chave + "_de", dataInicial); // envia como DateTime
                        wherePesquisa.AppendLine(" AND CONVERT(DATE, " + campo.chave + ") >= @" + campo.chave + "_de");
                    }

                    string dataFinal = ("" + partes[1]).Trim();
                    if (!dataFinal.Equals(""))
                    {
                        _conexao.prm("@" + campo.chave + "_ate", dataFinal);
                        wherePesquisa.AppendLine(" AND CONVERT(DATE, " + campo.chave + ") <= @" + campo.chave + "_ate");
                    }

                    wherePesquisa.AppendLine(" OR " + campo.chave + " is Null )");

                    /*
                     Bug a corrigir: Ele tras o filtro de pesquisa baseado na data mas traz o null também.
                    Precisa pensar num jeito de quando as datas forem as extremas ele filtrar com o is null;
                    se não for os extremos filtrar apenas as datas;
                    */
                }

                else
                {
                    _conexao.prm("@" + campo.chave, campo.valor);
                    wherePesquisa.AppendLine(" AND " + campo.chave + " = @" + campo.chave);
                }
            }


            if (campo.tipoComponente == typeof(CompTextBox))
            {
                if (campo.valorMonetario && !campo.valor.Equals(""))
                    camposPesquisa.AppendLine(" ,FORMAT(" + campo.chave + ", 'C', 'pt-BR') as  " + campo.chave);
                else
                    camposPesquisa.AppendLine(" ," + campo.chave);
            }
            else if (campo.tipoComponente == typeof(CompCalendario))
                camposPesquisa.AppendLine(" ," + campo.chave);
            else if (campo.tipoComponente == typeof(CompDropdowlist))
                camposPesquisa.AppendLine(" ," + campo.chaveExtra);
            else if (campo.tipoComponente == typeof(CompFileUpload))
                camposPesquisa.AppendLine(" ," + campo.chaveExtra);
        }

        return _conexao.DataTable(@"SELECT
        " + nautaSQL.primaryKey + " " + nautaSQL.selectorFrom.Replace("*", "") + camposPesquisa.ToString()
        + @" FROM " +
        nautaSQL.locationSelect
        + @" WHERE 1 = 1" + wherePesquisa.ToString());
    }

    public NautaBuild AdicionarDados(NautaModelSQL nautaSQL, string mensagemRetornoPositivo = "Registro inserido com sucesso!")
    {
        NautaBuild debug = new NautaBuild();
        if (this.camposSQL.Count == 0)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Nenhum valor inserido, nenhum componente listado!";
        }
        else
        {
            TransactSQL tInsert = new TransactSQL();
            foreach (CampoSQL campo in this.camposSQL)
            {
                tInsert.add(campo.chave, campo.valor, campo.ehTexto);
            }

            tInsert.insert(nautaSQL.locationInsert);

            try
            {
                debug.Sucesso = true;
                debug.Mensagem = mensagemRetornoPositivo;
                tInsert.exec();
            }
            catch (Exception ex)
            {
                debug.Sucesso = false;
                debug.RetornoDesenvolvimento = ex.ToString();
                debug.Mensagem = "Houve um erro ao tentar adicionar os dados, entre em contato com a equipe de suporte";
            }

        }
        return debug;
    }

    public NautaBuild EditarDados(NautaModelSQL nautaSQL, string mensagemRetornoPositivo = "Registro atualizado com sucesso!")
    {
        NautaBuild debug = new NautaBuild();
        if (this.camposSQL.Count == 0)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Nenhum valor atualizado, nenhum componente listado!";
        }
        else
        {
            if (nautaSQL.idClient.ToString().Equals(""))
            {
                debug.Sucesso = false;
                debug.Mensagem = "Valor de identificação do registro não encontrado!";
            }
            else
            {
                TransactSQL tUpdate = new TransactSQL();
                foreach (CampoSQL campo in this.camposSQL)
                {
                    tUpdate.add(campo.chave, campo.valor);
                }

                tUpdate.where(nautaSQL.primaryKey, nautaSQL.idClient.ToString());
                tUpdate.update(nautaSQL.locationUpdate);

                try
                {
                    debug.Sucesso = true;
                    debug.Mensagem = mensagemRetornoPositivo;
                    tUpdate.exec();
                }
                catch (Exception ex)
                {
                    debug.Sucesso = false;
                    debug.RetornoDesenvolvimento = ex.ToString();
                    debug.Mensagem = "Houve um erro ao tentar editar os dados, entre em contato com a equipe de suporte";
                }
            }
        }
        return debug;
    }

    public NautaBuild ExcluirDados(NautaModelSQL nautaSQL, string mensagemRetornoPositivo = "Registro excluido com sucesso!")
    {
        NautaBuild debug = new NautaBuild();
        if (nautaSQL.idClient.ToString().Equals(""))
        {
            debug.Sucesso = false;
            debug.Mensagem = "Valor de identificação do registro não encontrado!";
        }
        else
        {
            TransactSQL tDelete = new TransactSQL();
            tDelete.where(nautaSQL.primaryKey, nautaSQL.idClient.ToString());
            tDelete.delete(nautaSQL.locationDelete);

            try
            {
                debug.Sucesso = true;
                debug.Mensagem = mensagemRetornoPositivo;
                tDelete.exec();
            }
            catch (Exception ex)
            {
                debug.Sucesso = false;
                debug.RetornoDesenvolvimento = ex.ToString();
                debug.Mensagem = "Houve um erro ao tentar excluir os dados, entre em contato com a equipe de suporte";
            }
        }

        return debug;
    }

    public DataRow RetornaDadosFormularioEdicao(NautaModelSQL nautaSQL)
    {
        nautaSQL.where = nautaSQL.where.Equals("") ? "1 = 1" : nautaSQL.where;

        string sqlQuery = @"
        SELECT
        " + nautaSQL.selectorFrom + @"
        FROM " + nautaSQL.table + @"
        WHERE " + nautaSQL.primaryKey + @" = @" + nautaSQL.primaryKey + @"
        AND " + nautaSQL.where + @"
        Order By " + nautaSQL.orderBy;

        _conexao.prm("@" + nautaSQL.primaryKey, nautaSQL.idClient.ToString());
        return _conexao.DataRow(sqlQuery);
    }

    public DataRow RetornaDadosFormularioExibicao(NautaModelSQL nautaSQL)
    {
        nautaSQL.where = nautaSQL.where.Equals("") ? "1 = 1" : nautaSQL.where;

        string sqlQuery = @"
        SELECT
        " + nautaSQL.selectorFrom + @"
        FROM " + nautaSQL.table + @"
        WHERE " + nautaSQL.primaryKey + @" = @" + nautaSQL.primaryKey + @"
        AND " + nautaSQL.where + @"
        Order By " + nautaSQL.orderBy;

        _conexao.prm("@" + nautaSQL.primaryKey, nautaSQL.idClient.ToString());
        return _conexao.DataRow(sqlQuery);
    }



}