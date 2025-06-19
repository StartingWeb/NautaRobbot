using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class NautaRepository
{
    public readonly SQL _conexao;
    public List<CamposTransactSQL> camposTransactSQLs = new List<CamposTransactSQL>();

    public class CamposTransactSQL
    {
        public string chave;
        public string valor;
    }

    public NautaRepository()
    {
        _conexao = new SQL();
    }

    public NautaBuild EditarDados(NautaModelSQL nautaSQL)
    {
        NautaBuild debug = new NautaBuild();
        if (this.camposTransactSQLs.Count == 0)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Nenhum valor atualizado, nenhum componente listado!";
        }
        else
        {
            if(nautaSQL.idClient.ToString().Equals(""))
            {
                debug.Sucesso = false;
                debug.Mensagem = "Valor de identificação do registro não encontrado!";
            }
            else
            {
                TransactSQL tUpdate = new TransactSQL();
                foreach (CamposTransactSQL campo in this.camposTransactSQLs)
                {
                    tUpdate.add(campo.chave, campo.valor);
                }

                tUpdate.where(nautaSQL.primaryKey, nautaSQL.idClient.ToString());
                tUpdate.update(nautaSQL.table);

                try
                {
                    debug.Sucesso = true;
                    debug.Mensagem = "Dados atualizados com sucesso!";
                    tUpdate.exec();
                }
                catch(Exception ex)
                {
                    debug.Sucesso = false;
                    debug.RetornoDesenvolvimento = ex.ToString();
                    debug.Mensagem = "Houve um erro ao tentar editar os dados, entre em contato com a equipe de suporte";
                }
            }
        }
        return debug;
    }



    public DataRow RetornaDadosFormularioEdicao(NautaModelSQL sql)
    {
        sql.where = sql.where.Equals("") ? "1 = 1" : sql.where;

        string sqlQuery = @"
        SELECT
        " + sql.selectorFrom + @"
        FROM " + sql.table + @"
        WHERE " + sql.primaryKey + @" = @" + sql.primaryKey + @"
        AND " + sql.where + @"
        Order By " + sql.orderBy;

        _conexao.prm("@" + sql.primaryKey, sql.idClient.ToString());
        return _conexao.DataRow(sqlQuery);
    }



}