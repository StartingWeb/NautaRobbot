using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
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
            else if (componente.GetType() == typeof(CompFileUpload))
            {

            }
            else if (componente.GetType() == typeof(CompCalendario))
            {
                if (panel.ID.Contains("Pesquisa"))
                {
                    NameValueCollection Form = HttpContext.Current.Request.Form;
                    string prefixo = "ctl00$body$";
                    string dataInicio = Form[prefixo + componenteBase.SQL.campoSQL + "_De"];
                    string dataFim = Form[prefixo + componenteBase.SQL.campoSQL + "_Ate"];

                    if (!dataInicio.Equals(""))
                        dataInicio = DateTime.Parse(dataInicio).ToString();
                    else dataInicio = DateTime.Parse("01/01/1900").ToString();

                    if (!dataFim.Equals(""))
                        dataFim = DateTime.Parse(dataFim).ToString();
                    else dataFim = DateTime.Parse("01/01/2500").ToString();

                    valorComponente = dataInicio + "," + dataFim;
                }
                else
                {
                    TextBox calendario = (TextBox)panel.FindControl(componenteBase.SQL.campoSQL);
                    if (calendario != null) valorComponente = calendario.Text;
                }
            }
            else if (componente.GetType() == typeof(CompDropdowlist))
            {
                DropDownList dropDownList = (DropDownList)panel.FindControl(componenteBase.SQL.campoSQL);
                if (dropDownList != null) valorComponente = dropDownList.SelectedValue;
            }
        }
        return valorComponente;
    }

    public string RecuperarValorComponenteFileUpload(Panel panel, CompFileUpload componente)
    {
        string valorComponente = "";
        CompFileUpload fileUpload = (CompFileUpload)componente;
        FileUpload fileUploadPanel = (FileUpload)panel.FindControl(componente.SQL.campoSQL);

        //Nota: Colocar um tratamento aqui se começar a dar erro!
        if (fileUploadPanel.HasFile)
        {
            fileUpload.salvarArquivo(fileUploadPanel, fileUpload.SavePathImage);
            valorComponente = fileUpload.SavePathImage + fileUploadPanel.FileName;
        }

        return valorComponente;
    }

    public void RedirectFormularioInicial()
    {
        string url = HttpContext.Current.Request.Url.AbsolutePath.ToString() ?? "";
        HttpContext.Current.Response.Redirect(url);
    }

    public void ReloadPage()
    {
        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
    }

    public string ConverteBotaoEmHtml(Button button)
    {
        using (StringWriter sw = new StringWriter())
        {
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            button.RenderControl(hw);
            return sw.ToString();
        }
    }

    public string RetornaBotoesListagemPadrao(int idRegistro, bool ocultarBotaoEditarListagem, bool ocultarBotaoExibirListagem)
    {
        Button btnEditar = new Button();
        btnEditar.Text = "Editar";
        btnEditar.CssClass = "botaoActionPadrao";
        btnEditar.OnClientClick = "window.location.href='?modo=e&id=" + idRegistro + "'; return false;";

        Button btnVer = new Button();
        btnVer.Text = "Exibir";
        btnVer.CssClass = "botaoActionPadraoExibir";
        btnVer.OnClientClick = "window.location.href='?modo=v&id=" + idRegistro + "'; return false;";

        string retorno = "";

        if (!ocultarBotaoEditarListagem) retorno += ConverteBotaoEmHtml(btnEditar);
        if (!ocultarBotaoExibirListagem) retorno += ConverteBotaoEmHtml(btnVer);

        return retorno;
    }


}

