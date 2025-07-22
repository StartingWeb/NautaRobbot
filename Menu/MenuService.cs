using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NautaRobbot.Helpers;

namespace NautaRobbot.Menu
{
    public class MenuService
    {
        public readonly MenuRepository _repository;

        public MenuService()
        {
            _repository = new MenuRepository();
        }

        public List<MenuModel> RetornaMenuModel(int id_usuario)
        {
            List<MenuModel> listaMenus = new List<MenuModel>();
            DataTable dados = _repository.RetornaMenuModulo(id_usuario);

            //Menus Principais (Pai)
            DataRow[] menusPai = dados.Select("id_menu_pai is NULL", "int_ordem");
            foreach (DataRow linha in menusPai)
            {
                int id_menu = Fac.convertInt(linha["id_menu"].ToString() ?? "");

                if (!listaMenus.Where(calback => calback.id_menu == id_menu).Any())
                {
                    listaMenus.Add(new MenuModel
                    {
                        id_menu = id_menu,
                        chr_link = linha["chr_link"].ToString() ?? "",
                        chr_menu = linha["chr_menu"].ToString() ?? "",
                        fl_status = linha["fl_status"].ToString() ?? "",
                        listSubMenus = new List<MenuModel>()
                    });
                }
            }

            //Menus Filhos
            DataRow[] menusFilho = dados.Select("id_menu_pai is not NULL", "int_ordem");
            foreach (DataRow linha in menusFilho)
            {
                int id_menu_pai = Fac.convertInt(linha["id_menu_pai"].ToString() ?? "");
                MenuModel menuPai = listaMenus.FirstOrDefault(c => c.id_menu == id_menu_pai);
                int indexMenuPai = listaMenus.FindIndex(m => m.id_menu == id_menu_pai);

                if (menuPai != null) 
                {
                    listaMenus[indexMenuPai].listSubMenus.Add(new MenuModel
                    {
                        chr_menu = linha["chr_menu"].ToString() ?? "",
                        chr_link = linha["chr_link"].ToString() ?? "",
                        fl_status = linha["fl_status"].ToString() ?? "",
                        id_menu_pai = id_menu_pai,
                    });
                }
            }

            return listaMenus;
        }

        public bool ValidarAcessoPagina(int id_usuario, string pathUrl)
        {
            bool validador = false;
            DataRow source = _repository.RetornaAcessoPagina(id_usuario, pathUrl);
            if (source != null) validador = true;
            return validador;
        }
    }
}
