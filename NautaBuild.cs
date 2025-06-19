using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NautaBuild
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public string RetornoDesenvolvimento { get; set; }
    public bool RedirecionamentoInicial { get; set; } = false;
    public List<string> Erros { get; set; }

}

