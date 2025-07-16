using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NautaRobbot;

using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using NautaRobbot.Helpers;

public class CompTextBox : CompBase
{
    public string Type { get; set; } = "text";
    public string PlaceHolder { get; set; } = "Digite um valor aqui...";
    public string Valor { get; set; } = "";
    public string CssClass { get; set; } = "";
    public bool ValorMonetario { get; set; } = false;

    TextBox RetornaComponente(CompTextBox componente)
    {
        TextBox txt = new TextBox();
        txt.ID = componente.SQL.campoSQL;
        txt.Text = componente.ValorMonetario && componente.Valor != "" ?
            Fac.convertMonetary(Fac.convertDouble(componente.Valor)) :
            componente.Valor;

        txt.ClientIDMode = ClientIDMode.Static;
        txt.Attributes.Add("Name", componente.SQL.campoSQL);
        txt.Attributes.Add("Type", componente.Type);
        txt.Attributes.Add("placeHolder", componente.PlaceHolder);
        txt.CssClass = componente.CssClass + " " + (componente.ValorMonetario ? "valor" : "");

        if (!componente.HTML.atributo.Equals("") && !componente.HTML.atributoValor.Equals(""))
            txt.Attributes.Add(componente.HTML.atributo, componente.HTML.atributoValor);

        return txt;
    }


    public Panel MontarComponente(CompTextBox componente, bool exibirTagObrigatoria = false)
    {
        var panel = new Panel();

        componente.CssClass = !componente.CssClass.Equals("") ?
            "campo-padrao " + componente.CssClass : "campo-padrao";

        panel.Attributes.Add("class", "separator_" + componente.SQL.campoSQL + " mb-3");

        //Label
        panel.Controls.Add(new LiteralControl(@"<div class=""titulo-padrao"">"
        + componente.HTML.label +
        (componente.Config.campoObrigatorio && exibirTagObrigatoria ? "*" : "")
        +
        "</div>"));

        //Componente
        panel.Controls.Add(RetornaComponente(componente));
        return panel;
    }

    public Panel MontarComponenteExibicao(CompTextBox componente)
    {
        var panel = new Panel();
        componente.CssClass = !componente.CssClass.Equals("") ?
           "campo-padrao " + componente.CssClass : "campo-padrao";

        panel.Attributes.Add("class", "separator_" + componente.SQL.campoSQL + " mb-3");

        //Label
        panel.Controls.Add(new LiteralControl(@"<b><span id=""campo_exibir_title_" + componente.SQL.campoSQL + @""" class=""exibir_padrao_title"">"));
        panel.Controls.Add(new LiteralControl(componente.HTML.label));
        panel.Controls.Add(new LiteralControl(@":</span></b>"));
        panel.Controls.Add(new LiteralControl(@"<span id=""campo_exibir_value_" + componente.SQL.campoSQL + @""" class=""exibir_padrao_value"">"));
        panel.Controls.Add(new LiteralControl(componente.Valor));
        panel.Controls.Add(new LiteralControl(@"</span>"));
        return panel;
    }

}

