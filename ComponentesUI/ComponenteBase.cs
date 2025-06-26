using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NautaRobbot
{
    public class CompBase
    {
        public CompBase_HTML HTML { get; set; } = new CompBase_HTML();
        public CompBase_SQL SQL { get; set; } = new CompBase_SQL();
        public CompBase_Config Config { get; set; } = new CompBase_Config();
        public CompBase_Modulos Modulos { get; set; } = new CompBase_Modulos();
    }

    public class CompBase_HTML
    {
        public string id { get; set; }
        public string label { get; set; }
        public string atributo { get; set; } = "";
        public string atributoValor { get; set; } = "";
    }

    public class CompBase_SQL
    {
        public string campoSQL { get; set; } = "";
        public string valorPadrao { get; set; } = "";
        public bool acaoSQLInsert { get; set; } = false;
        public bool acaoSQLUpdate { get; set; } = false;

    }

    public class CompBase_Config
    {
        public bool contadorNumerico { get; set; } = false;
        public bool contadorMonetario { get; set; } = false;
        public bool campoObrigatorio { get; set; } = false;
    }

    /// <summary>
    /// Pesquisar, Editar, Inserir, Listagem, Exibir
    /// </summary>
    public class CompBase_Modulos
    {
        public bool Pesquisar { get; set; } = false;
        public bool Editar { get; set; } = false;
        public bool Inserir { get; set; } = false;
        public bool Listagem { get; set; } = false;
        public bool Exibir { get; set; } = false;
    }

}
