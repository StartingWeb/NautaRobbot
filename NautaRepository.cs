using System;
using System.Collections.Generic;
using System.Data;
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
                _conexao.prm("@" + campo.chave, campo.valor);

                string comparativoWhere = "@" + campo.chave;
                if (campo.tipoComponente == typeof(CompTextBox))
                    comparativoWhere = " like '%'+ " + comparativoWhere + "+'%'";
                else
                    comparativoWhere = " = " + comparativoWhere;

                wherePesquisa.AppendLine(" AND " + campo.chave + comparativoWhere);
            }

            if (campo.valorMonetario)
                campo.chave = " ,FORMAT(" + campo.chave + ", 'C', 'pt-BR') as  " + campo.chave + " ";

            camposPesquisa.AppendLine(" ," + campo.chave);
        }

        return _conexao.DataTable(@"SELECT
        " + nautaSQL.primaryKey + camposPesquisa.ToString()
        + @" FROM " +
        nautaSQL.locationSelect
        + @" WHERE 1 = 1" + wherePesquisa.ToString());
    }

    public NautaBuild AdicionarDados(NautaModelSQL nautaSQL, string mensagemRetornoPositivo = "Dados inseridos com sucesso!")
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

    public NautaBuild EditarDados(NautaModelSQL nautaSQL, string mensagemRetornoPositivo = "Dados atualizados com sucesso!")
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