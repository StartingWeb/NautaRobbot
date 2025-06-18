using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NautaRobbot
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
    }
}
