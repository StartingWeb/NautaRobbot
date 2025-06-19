using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NautaRobbot;

public class NautaUiBuilder
{
    public bool isPostBack;
    NameValueCollection Form = HttpContext.Current.Request.Form;

    enum tipoExibicaoPanel
    {
        Pesquisar,
        Editar,
        Inserir,
        Exibir
    }

    bool ValidarModoExibicaoComponente(CompBase componenteBase, tipoExibicaoPanel modoExibicao)
    {
        bool validacao = false;

        if (tipoExibicaoPanel.Pesquisar == modoExibicao
            && componenteBase.Modulos.Pesquisar)
            validacao = true;
        else if (tipoExibicaoPanel.Editar == modoExibicao
            && componenteBase.Modulos.Editar)
            validacao = true;
        else if (tipoExibicaoPanel.Inserir == modoExibicao
           && componenteBase.Modulos.Inserir)
            validacao = true;
        else if (tipoExibicaoPanel.Exibir == modoExibicao
          && componenteBase.Modulos.Exibir)
            validacao = true;

        return validacao;
    }


    private Panel MontarComponentes(List<CompBase> ListComponentesUI,
        tipoExibicaoPanel modoExibicao,
        DataRow dadosCarregados = null)
    {
        List<CompBase> componentesExistentes = new List<CompBase>();
        
        Panel panelContainer = new Panel();
        panelContainer.Attributes.Add("class", "container-fluid");

        Panel panelRow = new Panel();
        panelRow.Attributes.Add("class", "row");


        foreach (var componenteBase in ListComponentesUI)
        {
            if (ValidarModoExibicaoComponente(componenteBase, modoExibicao))
            {
                string valorComponente = "";
                panelRow.Controls.Add(new LiteralControl(@"<div class=""col-lg-3"">"));
                Panel componenteCriado = new Panel();

                componentesExistentes.Add(componenteBase); //Validação para não duplicar campos na tela, mesmo que venha da declarativa

                if (componenteBase.GetType() == typeof(CompTextBox))
                {
                    CompTextBox textBox = (CompTextBox)componenteBase;

                    if (modoExibicao == tipoExibicaoPanel.Editar)
                    {
                        if (dadosCarregados.Table.Columns.Contains(textBox.SQL.campoSQL))//Aqui ele mantem o filtro de pesquisa quando clica em pesquisar
                            valorComponente =  dadosCarregados[textBox.SQL.campoSQL].ToString() ?? "";
                    }
                    else if (modoExibicao == tipoExibicaoPanel.Pesquisar && isPostBack)
                    {
                        valorComponente = Form[textBox.SQL.campoSQL];
                    }

                    textBox.Valor = valorComponente;
                    componenteCriado = textBox.MontarComponente(textBox);
                }

                panelRow.Controls.Add(componenteCriado);
                panelRow.Controls.Add(new LiteralControl(@"</div>"));
            }
        }

        panelContainer.Controls.Add(panelRow);
        return panelContainer;
    }

    public Panel MontarUIFormularioPesquisa(List<CompBase> ListComponentesUI, NautaModelFormulario configFormulario)
    {
        var panel = new Panel();
        //Cabeçalho - INI
        panel.Controls.Add(new LiteralControl(@"<div id=""div_container_cabecalho"" class=""container-fluid container-cabecalho-padrao"">"));
        panel.Controls.Add(new LiteralControl(@"    <div class=""row"">"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-5"">"));
        panel.Controls.Add(new LiteralControl(@"            <span>" + configFormulario.tituloPrincipal + "</span>"));
        panel.Controls.Add(new LiteralControl(@"            <h2><b>" + configFormulario.subTitulo + "</b></h2>"));
        panel.Controls.Add(new LiteralControl(@"        </div>"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-4""></div>"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-3"">"));

        if(configFormulario.ocultarBotaoAdicionar)
        {
            panel.Controls.Add(new LiteralControl(@"        <button class=""botaoPrimaryPadrao"" 
                                                            onclick=""window.location.href='?modo=i'; return false;"">
                                                                Adicionar
                                                            </button>"));
        }

        panel.Controls.Add(new LiteralControl(@"        </div>"));
        panel.Controls.Add(new LiteralControl(@"    </div>"));
        panel.Controls.Add(new LiteralControl(@"</div>"));
        //Cabeçalho - FIM


        panel.Controls.Add(new LiteralControl(@"<fieldSet class=""mob-field-pesquisar"">"));
        panel.Controls.Add(new LiteralControl(@"    <h4><b>Formulário de Pesquisa</b></h4>"));
        panel.Controls.Add(new LiteralControl(@"    <hr/>"));
        panel.Controls.Add(MontarComponentes(ListComponentesUI, tipoExibicaoPanel.Pesquisar));
        panel.Controls.Add(new LiteralControl(@"    <div class=""container-fluid mt-3"">"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""row mob-row-buttons"">"));
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-left"">"));
        
        //Button cancelar
        Button btnCancelar = new Button();
        btnCancelar.ID = "btnCancelarPesquisa";
        btnCancelar.Text = "Limpar";
        btnCancelar.Click += configFormulario.PesquisarClick_cancelar;
        btnCancelar.Attributes.Add("class", "botaoSecondPadrao");
        panel.Controls.Add(btnCancelar);

        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-left"">"));
        
        Button btnPesquisar = new Button();
        btnPesquisar.ID = "btnPesquisar";
        btnPesquisar.Text = "Pesquisar";
        btnPesquisar.Click += configFormulario.PesquisarClick;
        btnPesquisar.Attributes.Add("class", "botaoSuccessPadrao");
        panel.Controls.Add(btnPesquisar);
        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"        </div>"));//Row- Fim
        panel.Controls.Add(new LiteralControl(@"    </div>"));//Container- Fim
        panel.Controls.Add(new LiteralControl(@"</fieldset>"));
        return panel;
    }

    public Panel MontarUIFormularioEdicao(List<CompBase> ListComponentesUI, DataRow dadosCarregados, NautaModelFormulario configFormulario)
    {
        var panel = new Panel();

        //Cabeçalho - INI
        panel.Controls.Add(new LiteralControl(@"<div id=""div_container_cabecalho"" class=""container-fluid container-cabecalho-padrao"">"));
        panel.Controls.Add(new LiteralControl(@"    <div class=""row"">"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-5"">"));
        panel.Controls.Add(new LiteralControl(@"            <span>" + configFormulario.tituloPrincipal + "</span>"));
        panel.Controls.Add(new LiteralControl(@"            <h2><b>" + configFormulario.subTitulo + "</b></h2>"));
        panel.Controls.Add(new LiteralControl(@"        </div>"));

        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-4""></div>"));

        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-3""></div>"));
        panel.Controls.Add(new LiteralControl(@"    </div>"));
        panel.Controls.Add(new LiteralControl(@"</div>"));
        //Cabeçalho - FIM


        panel.Controls.Add(new LiteralControl(@"<fieldSet>"));
        panel.Controls.Add(new LiteralControl(@"    <h4><b>Formulário de Edição</b></h4>"));
        panel.Controls.Add(new LiteralControl(@"    <hr/>"));
        panel.Controls.Add(MontarComponentes(ListComponentesUI, tipoExibicaoPanel.Editar, dadosCarregados));
        panel.Controls.Add(new LiteralControl(@"    <div class=""container-fluid mt-3"">"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""row mob-row-buttons"">"));

        //Botão Cancelar
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-left"">"));
        if (!configFormulario.ocultarBotaoCancelarEditar)
        {
            Button btnCancelar = new Button();
            btnCancelar.ID = "btnCancelarEdicao";
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += configFormulario.EditarClick_cancelar;
            btnCancelar.Attributes.Add("class", "botaoSecondPadrao");
            btnCancelar.Attributes.Add("Title", "Cancelar");
            panel.Controls.Add(btnCancelar);
        }

        //Botão editar
        Button btnEditar = new Button();
        btnEditar.ID = "btnGravarEdicao";
        btnEditar.Text = "Gravar";
        btnEditar.Click += configFormulario.EditarClick;
        btnEditar.Attributes.Add("class", "botaoSuccessPadrao");
        btnEditar.Attributes.Add("Title", "Gravar");
        panel.Controls.Add(btnEditar);

        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"        </div>")); //Row - Fim
        panel.Controls.Add(new LiteralControl(@"    </div>")); //Container - Fim
        panel.Controls.Add(new LiteralControl(@"</fieldset>")); //Container - Fim

        return panel;
    }

  

}
