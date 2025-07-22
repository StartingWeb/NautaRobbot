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

public class CompCalendario : CompBase
{
    public string PlaceHolder { get; set; } = "Digite um valor aqui...";
    public string Valor { get; set; } = "";
    public string CssClass { get; set; } = "";


    Panel RetornaComponentePanelPesquisar(CompCalendario componente)
    {
        Panel panel = new Panel();
        panel.CssClass = "container-fluid";

        Panel row = new Panel();
        row.CssClass = "row";

        Panel col1 = new Panel();
        col1.CssClass = "col-lg-6 d-flex align-items-center justify-content-center p-0";
        col1.Controls.Add(new LiteralControl(@"<div class='subTituloPadrao'>De:</div>"));
        col1.Controls.Add(RetornaComponente(componente, componente.SQL.campoSQL + "_De"));
        row.Controls.Add(col1);

        Panel col2 = new Panel();
        col2.CssClass = "col-lg-6 d-flex align-items-center justify-content-center p-0";
        col2.Controls.Add(new LiteralControl(@"<div class='subTituloPadrao subTituloPadraoAte'>Até:</div>"));
        col2.Controls.Add(RetornaComponente(componente, componente.SQL.campoSQL + "_Ate"));
        row.Controls.Add(col2);
        panel.Controls.Add(row);
        return panel;
    }

    TextBox RetornaComponente(CompCalendario componente, string idHTML = "")
    {
        TextBox calendario = new TextBox();
        calendario.ID = idHTML.Equals("") ? componente.SQL.campoSQL : idHTML;

        if (!string.IsNullOrWhiteSpace(componente.Valor))
        {
            if (DateTime.TryParse(componente.Valor.Split(' ')[0], out DateTime data))
            {
                // Formata no padrão aceito pelo input type="date"
                calendario.Text = data.ToString("yyyy-MM-dd");
            }
            else
            {
                // Se não for uma data válida, atribui como está (opcional)
                calendario.Text = componente.Valor;
            }
        }

        if (!componente.HTML.atributo.Equals("") && !componente.HTML.atributoValor.Equals(""))
            calendario.Attributes
                .Add(componente.HTML.atributo, componente.HTML.atributoValor);


        calendario.ClientIDMode = ClientIDMode.Static;
        calendario.Attributes.Add("Name", componente.SQL.campoSQL);
        calendario.Attributes.Add("Type", "date");
        calendario.Attributes.Add("placeHolder", componente.PlaceHolder);
        calendario.CssClass = componente.CssClass;

        return calendario;
    }


    public Panel MontarComponente(CompCalendario componente,
        bool exibirTagObrigatoria = false,
        bool moduloExibicao = false,
        bool moduloPesquisa = false)
    {
        var panel = new Panel();
        if (moduloExibicao)
        {
            panel.Controls.Add(MontarComponenteExibicao(componente));
        }
        else
        {
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
            if (moduloPesquisa)
                panel.Controls.Add(RetornaComponentePanelPesquisar(componente));
            else
                panel.Controls.Add(RetornaComponente(componente));
        }
        return panel;
    }

    public Panel MontarComponenteExibicao(CompCalendario componente)
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

