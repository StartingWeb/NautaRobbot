using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NautaRobbot;

public class Ambiente
{
    string connectionString = "Data Source={dataSource};Initial Catalog={dataBase};User ID={userSQL};Password={passwordSQL};";
    internal enum tipoAmbiente
    {
        Main,
        Teste,
        Localhost
    }

    public Ambiente()
    {
        tipoAmbiente ambiente = RetornarAmbienteAtual(Inicializador.urlHttpApplication);

        if (ambiente != tipoAmbiente.Localhost)
            Inicializador.dataBase += ambiente == tipoAmbiente.Teste ? "_T" : "_P";

        connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};",
              Inicializador.dataSource,
              Inicializador.dataBase,
              Inicializador.userSQL,
              Inicializador.passwordSQL);
    }

    internal tipoAmbiente RetornarAmbienteAtual(string urlHttpApplication)
    {
        if (urlHttpApplication.Contains("localhost"))
            return tipoAmbiente.Localhost;
        else if (urlHttpApplication.Contains("teste."))
            return tipoAmbiente.Teste;
        else return tipoAmbiente.Main;
    }

    public string ObterConnectionString()
    {
        return connectionString;
    }
}

