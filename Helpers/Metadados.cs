using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NautaRobbot.Helpers
{
    public class Metadados<T>
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public T Dados { get; set; }
        public List<string> Erros { get; set; }

        public Metadados()
        {
            Erros = new List<string>();
        }

    }
}
