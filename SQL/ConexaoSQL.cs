using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NautaRobbot;

public class ConexaoSQL
{
    public readonly Ambiente _ambiente;
    public SqlConnection sqlConnection;
    public List<object> Parameters = new List<object>();

    public ConexaoSQL()
    {
        _ambiente = new Ambiente();
        sqlConnection = Open();
        Parameters = new List<object>();
    }

    public SqlConnection Open()
    {
        string str_conn = "";
        str_conn = _ambiente.ObterConnectionString();
        SqlConnection conn = new SqlConnection(str_conn);
        return conn;
    }

    public SqlCommand Command(string str_sql)
    {
        SqlCommand command = new SqlCommand(str_sql, sqlConnection);

        if (Parameters != null)
        {
            foreach (dynamic parameter in Parameters)
            {
                string chr_campo = parameter.chr_campo;
                string chr_valor = parameter.chr_valor;

                command.Parameters.AddWithValue(chr_campo, chr_valor);
            }
        }
        return command;
    }
}

