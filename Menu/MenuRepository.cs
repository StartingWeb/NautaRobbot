using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NautaRobbot.Menu
{
    public class MenuRepository
    {
        public readonly SQL _conexao;

        public MenuRepository()
        {
            _conexao = new SQL();
        }

        public DataTable RetornaMenuModulo(int id_usuario)
        {
            _conexao.prm("@id_usuario", id_usuario.ToString());
            return _conexao.DataTable(@"SELECT
            DISTINCT id_menu,
            chr_menu, 
            chr_path_icon,
            chr_link,
            int_ordem,
            id_menu_pai, 
            id_menu,
            fl_status
            FROM vwAuthMenuPerfil
            WHERE id_perfil in (select id_perfil from AuthUsuarioPerfil where id_usuario = @id_usuario)
            AND (fl_status = 'a' or fl_status = 'o')
            ORDER BY int_ordem");
        }

        public DataRow RetornaAcessoPagina(int id_usuario, string pathUrl)
        {
            _conexao.prm("@id_usuario", id_usuario.ToString());
            _conexao.prm("@pathUrl", pathUrl.ToString());
            return _conexao.DataRow(@"
            SELECT
            TOP 1 id_menu
            From vwAuthMenuPerfil menuPerfil
            WHERE menuPerfil.id_perfil in (SELECT id_perfil from AuthUsuarioPerfil WHERE
                                id_usuario = @id_usuario)
            AND chr_link = @pathUrl
            AND (menuPerfil.fl_status = 'a' or menuPerfil.fl_status = 'o')"); //Ativo ou oculto
        }
    }
}
