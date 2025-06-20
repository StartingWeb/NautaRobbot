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
using System.Linq.Expressions;
using System.Data;

public class CompDropdowlist : CompBase
{
    private readonly CompDropdowlistRepository _repository;

    public CompDropdowlist()
    {
        _repository = new CompDropdowlistRepository();
    }

    public string CampoSqlListagem { get; set; } = "";
    public List<ListItem> ListagemFixa = new List<ListItem>();
    public string QuerySQL = "";


    public string PlaceHolder { get; set; } = "Digite um valor aqui...";
    public string Valor { get; set; } = "";
    public string CssClass { get; set; } = "";

    public bool PesquisaCampo = true;

    private DropDownList RetornaDropDownComDados(CompDropdowlist componente)
    {
        DropDownList dropDownList = new DropDownList();
        if (!componente.Config.campoObrigatorio)
            dropDownList.Items.Add(new ListItem { Text = "Selecione um valor...", Value = "" });

        if (!componente.QuerySQL.Equals(""))
        {
            //Carregar Dados
            DataTable dados = _repository.RetornaDadosLoadDropDown(componente.QuerySQL);
            foreach (DataRow dr in dados.Rows)
            {
                string valor = dr[0].ToString() ?? "";
                string texto = dr[1].ToString() ?? "";

                dropDownList.Items.Add(new ListItem { Text = texto, Value = valor });
            }

            if (componente.SQL.valorPadrao.Equals(""))
                dropDownList.SelectedValue = componente.SQL.valorPadrao;
        }
        else if (componente.ListagemFixa.Count > 0)
        {
            foreach (ListItem i in componente.ListagemFixa)
            {
                string valor = "" + i.Value;
                string texto = "" + i.Text;

                dropDownList.Items.Add(new ListItem { Text = texto, Value = valor });
            }
        }

        if (!componente.Valor.Equals(""))
            dropDownList.SelectedValue = componente.Valor;
        else if (componente.SQL.valorPadrao.Equals(""))
            dropDownList.SelectedValue = componente.SQL.valorPadrao;

        return dropDownList;
    }

    public DropDownList RetornaComponente(CompDropdowlist componente)
    {
        DropDownList drop = RetornaDropDownComDados(componente);
        drop.ID = componente.SQL.campoSQL;
        drop.Attributes.Add("placeHolder", componente.PlaceHolder);
        drop.ClientIDMode = ClientIDMode.Static;
        drop.CssClass = componente.CssClass + (componente.PesquisaCampo ? " selectpicker" : " campo-padrao ddl-padrao");
        drop.Attributes.Add("name", componente.SQL.campoSQL);
        drop.Attributes.Add("data-search", componente.PesquisaCampo ? "true" : "false");
        return drop;
    }

    public Panel MontarComponente(CompDropdowlist componente, bool exibirTagObrigatoria = false)
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

    public Panel MontarComponenteExibicao(CompDropdowlist componente)
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

public class CompDropdowlistRepository
{
    public readonly SQL _conexao;

    public CompDropdowlistRepository()
    {
        _conexao = new SQL();
    }

    public DataTable RetornaDadosLoadDropDown(string querySQL)
    {
        return _conexao.DataTable(querySQL);
    }
}

