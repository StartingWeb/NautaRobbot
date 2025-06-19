using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using NautaRobbot;

public class NautaHelper
{
    public string RecuperaValorComponentePanel(Panel panel, object componente)
    {
        string valorComponente = "";

        if (componente != null)
        {
            CompBase componenteBase = (CompBase)componente;

            if (componente.GetType() == typeof(CompTextBox))
            {
                TextBox textBox = (TextBox)panel.FindControl(componenteBase.SQL.campoSQL);
                if (textBox != null) valorComponente = textBox.Text;
            }
        }
        return valorComponente;
    }

    public void ReloadPage()
    {
        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
    }


}

