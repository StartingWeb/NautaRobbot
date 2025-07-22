using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NautaRobbot.Helpers
{
    public static class Fac
    {
        public static bool ValidarSenhaHash(string senha, string hashDataBase)
        {
            string hashSenhaAtual = BCrypt.Net.BCrypt.HashPassword(senha);
            return BCrypt.Net.BCrypt.Verify(senha, hashDataBase);
        }

        public static string RetornaSenhaHash(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public static int convertInt(string valor)
        {
            int retorno = 0;
            return int.TryParse(valor, out retorno) ? retorno : 0;
        }

        public static Double convertDouble(string valor)
        {
            Double retorno = 0;
            return Double.TryParse(valor, out retorno) ? retorno : 0;
        }

        public static string convertMonetary(double value)
        {
            return value.ToString("c");
        }
    }
}
