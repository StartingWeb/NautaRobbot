using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NautaRobbot.Menu
{
    public class MenuModel
    {
        public int id_menu { get; set; }
        public string chr_menu { get; set; }
        public string chr_link { get; set; }
        public int  int_ordem { get; set; }
        public int id_menu_pai { get; set; }
        public string fl_status { get; set; }

        public List<MenuModel> listSubMenus = new List<MenuModel>();
    }
}
