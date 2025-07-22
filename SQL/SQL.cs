using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

public class SQL
{
    public static ConexaoSQL _connection { get; set; }

    public SQL()
    {
        _connection = new ConexaoSQL();
    }

    public void Close()
    {
        _connection.sqlConnection.Close();
    }

    public void prm(string chr_campo, string chr_valor)
    {
        _connection.Parameters.Add(new
        {
            chr_campo = chr_campo,
            chr_valor = chr_valor
        });
    }

    public SqlDataReader DataReader(string str_sql)
    {
        SqlCommand command = _connection.Command(str_sql);

        if (_connection.sqlConnection.State != ConnectionState.Open)
            _connection.sqlConnection.Open();

        // Remove o finally com Close()
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }



    public DataTable DataTable(string str_sql)
    {
        DataTable dataTable = new DataTable();

        try
        {
            SqlCommand command = _connection.Command(str_sql);

            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                if (_connection.sqlConnection.State != ConnectionState.Open)
                    _connection.sqlConnection.Open();

                adapter.Fill(dataTable);
            }
        }
        finally
        {
            Close();
        }

        return dataTable;
    }


    public DataRow DataRow(string str_sql)
    {
        DataRow dr = null;
        DataTable dt = DataTable(str_sql);

        if (dt.Rows.Count > 0 && dt != null)
            dr = dt.Rows[0];

        return dr;
    }


}

