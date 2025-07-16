using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
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

    bool ValidarDuplicidadeComponentes(List<CompBase> componentesExistentes, CompBase componente)
    {
        return componentesExistentes.Any(c => c == componente);
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
            if (componenteBase.GetType() == typeof(CompFileUpload) &&
                modoExibicao == tipoExibicaoPanel.Pesquisar) continue;

            if (ValidarModoExibicaoComponente(componenteBase, modoExibicao)
                && !ValidarDuplicidadeComponentes(componentesExistentes, componenteBase)
               )
            {
                panelRow.Controls.Add(new LiteralControl(@"<div class=""col-lg-3"">"));
                componentesExistentes.Add(componenteBase); //Validação para não duplicar campos na tela, mesmo que venha da declarativa

                Panel componenteCriado = new Panel();
                string valorComponente = "";
                bool exibirTagObrigatoria = tipoExibicaoPanel.Inserir == modoExibicao || tipoExibicaoPanel.Editar == modoExibicao ? true : false;

                if (modoExibicao == tipoExibicaoPanel.Editar || modoExibicao == tipoExibicaoPanel.Exibir)
                {
                    if (dadosCarregados.Table.Columns.Contains(componenteBase.SQL.campoSQL))
                        valorComponente = dadosCarregados[componenteBase.SQL.campoSQL].ToString() ?? "";
                }

                if (componenteBase.GetType() == typeof(CompTextBox))
                {
                    CompTextBox textBox = (CompTextBox)componenteBase;

                    if (modoExibicao == tipoExibicaoPanel.Pesquisar && isPostBack)
                        valorComponente = Form[componenteBase.SQL.campoSQL]; //Aqui ele mantem o filtro de pesquisa quando clica em pesquisar

                    textBox.Valor = valorComponente;
                    componenteCriado = modoExibicao == tipoExibicaoPanel.Exibir ?
                        textBox.MontarComponenteExibicao(textBox) :
                        textBox.MontarComponente(textBox, exibirTagObrigatoria);
                }
                else if (componenteBase.GetType() == typeof(CompCalendario))
                {
                    CompCalendario calendario = (CompCalendario)componenteBase;
                    calendario.Valor = valorComponente;
                    componenteCriado = calendario.MontarComponente(calendario,
                        exibirTagObrigatoria,
                        modoExibicao == tipoExibicaoPanel.Exibir,
                        modoExibicao == tipoExibicaoPanel.Pesquisar);

                    panelRow.Controls.RemoveAt(panelRow.Controls.Count - 1); //Remove o controle que acabou de inserir
                    panelRow.Controls.Add(new LiteralControl(@"<div class=""col-lg-4"">"));
                }
                else if (componenteBase.GetType() == typeof(CompFileUpload))
                {
                    CompFileUpload fileUpload = (CompFileUpload)componenteBase;
                    fileUpload.Valor = valorComponente;
                    componenteCriado = fileUpload.MontarComponente(fileUpload,
                        exibirTagObrigatoria,
                        modoExibicao == tipoExibicaoPanel.Exibir,
                        (modoExibicao == tipoExibicaoPanel.Inserir || modoExibicao == tipoExibicaoPanel.Editar));

                    panelRow.Controls.RemoveAt(panelRow.Controls.Count - 1); //Remove o controle que acabou de inserir
                    panelRow.Controls.Add(new LiteralControl(@"<div class=""col-lg-4"">"));
                }
                else if (componenteBase.GetType() == typeof(CompDropdowlist))
                {
                    CompDropdowlist dropdowlist = (CompDropdowlist)componenteBase;
                    if (modoExibicao == tipoExibicaoPanel.Exibir)
                    {
                        if (dadosCarregados.Table.Columns.Contains(dropdowlist.CampoSqlListagem))
                            valorComponente = dadosCarregados[dropdowlist.CampoSqlListagem].ToString() ?? "";
                    }

                    dropdowlist.Valor = valorComponente;
                    componenteCriado = modoExibicao == tipoExibicaoPanel.Exibir ?
                        dropdowlist.MontarComponenteExibicao(dropdowlist) :
                        dropdowlist.MontarComponente(dropdowlist, exibirTagObrigatoria);
                }

                panelRow.Controls.Add(componenteCriado);
                panelRow.Controls.Add(new LiteralControl(@"</div>"));
            }
        }

        panelContainer.Controls.Add(panelRow);
        return panelContainer;
    }



    private GridView MontarDataGridViewListagem(List<CompBase> ListComponentesUI, DataTable dados)
    {
        GridView gridView = new GridView
        {
            ID = "gridViewPesquisa",
            CssClass = "gridViewPesquisaPadrao mob-grid-view-padrao",
            PageSize = 10,
            AutoGenerateColumns = false // Prevenir geração automática de colunas
        };

        //Adicionar coluna de contagem
        gridView.Columns.Add(new BoundField
        {
            DataField = "Contagem",
            HeaderText = "#",
            HtmlEncode = false,
        });

        // Adicionar coluna de ações e botão de ação
        gridView.Columns.Add(new BoundField
        {
            DataField = "Actions",
            HeaderText = "Ações",
            HtmlEncode = false,
        });

        foreach (CompBase componente in ListComponentesUI)
        {
            if (componente.Modulos.Listagem)
            {
                string dataField = "";
                dataField = componente.SQL.campoSQL;

                if (componente.GetType() == typeof(CompDropdowlist))
                {
                    CompDropdowlist dropdowlist = (CompDropdowlist)componente;
                    dataField = dropdowlist.CampoSqlListagem;
                }
                else if (componente.GetType() == typeof(CompFileUpload))
                {
                    CompFileUpload fileUpload = (CompFileUpload)componente;
                    dataField = fileUpload.CampoSqlListagem;
                }

                gridView.Columns.Add(new BoundField
                {
                    DataField = dataField,
                    HeaderText = componente.HTML.label,
                    HtmlEncode = false,
                });
            }
        }

        gridView.DataSource = dados;
        gridView.DataBind();
        gridView.PageIndex = 0;

        // Adicionar atributos data-label às células da tabela
        foreach (GridViewRow row in gridView.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    string headerText = gridView.Columns[i].HeaderText;
                    row.Cells[i].Attributes["data-label"] = headerText;

                    string resultText = gridView.Columns[i].ToString();
                    row.Cells[i].Text = row.Cells[i].Text.Replace(" 00:00:00", "");
                }
            }
        }

        return gridView;
    }


    //Métodos Publicos

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
        panel.Controls.Add(new LiteralControl(@"        <div class=""col-lg-3 d-flex justify-content-center align-items-center"">"));

        if (!configFormulario.ocultarBotaoAdicionar)
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
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-end"">"));

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

    public Panel MontarUIListagem(List<CompBase> ListComponentesUI, DataTable dadosPesquisados)
    {
        var panel = new Panel();

        panel.Controls.Add(new LiteralControl(@"<fieldSet class=""field-listagem-padrao mob-field-listagem"">"));
        panel.Controls.Add(new LiteralControl(@"    <h4><b>Resultados</b></h4>"));
        panel.Controls.Add(new LiteralControl(@"    <hr/>"));

        if (dadosPesquisados.Rows.Count > 0)
            panel.Controls.Add(MontarDataGridViewListagem(ListComponentesUI, dadosPesquisados));
        else
            panel.Controls.Add(new LiteralControl(@"<span class='sp_sem_resultado'>Nenhum Resultado encontrado!</span>"));

        panel.Controls.Add(new LiteralControl(@"    </fieldset>"));
        return panel;
    }

    public Panel MontarUIFormularioAdicionar(List<CompBase> ListComponentesUI, NautaModelFormulario configFormulario)
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
        panel.Controls.Add(new LiteralControl(@"    <h4><b>Formulário de Adição</b></h4>"));
        panel.Controls.Add(new LiteralControl(@"    <hr/>"));
        panel.Controls.Add(MontarComponentes(ListComponentesUI, tipoExibicaoPanel.Inserir));
        panel.Controls.Add(new LiteralControl(@"    <div class=""container-fluid mt-3"">"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""row mob-row-buttons"">"));
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-left"">"));

        if (!configFormulario.ocultarBotaoCancelarAdicionar)
        {
            Button btnCancelar = new Button();
            btnCancelar.ID = "btnCancelarAdicionar";
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += configFormulario.AdicionarClick_cancelar;
            btnCancelar.Attributes.Add("class", "botaoSecondPadrao");
            btnCancelar.Attributes.Add("Title", "Cancelar");
            panel.Controls.Add(btnCancelar);
        }

        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-end"">"));

        Button btnEditar = new Button();
        btnEditar.ID = "btnAdicionar";
        btnEditar.Text = "Adicionar";
        btnEditar.Click += configFormulario.AdicionarClick;
        btnEditar.Attributes.Add("class", "botaoSuccessPadrao");
        panel.Controls.Add(btnEditar);

        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"        </div>")); //Row - Fim
        panel.Controls.Add(new LiteralControl(@"    </div>")); //Container - Fim
        panel.Controls.Add(new LiteralControl(@"</fieldset>")); //Fieldset - Fim

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

        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-end"">"));

        if (!configFormulario.ocultarBotaoExcluirEditar)
        {
            Button btnExcluir = new Button();
            btnExcluir.ID = "btnExcluir";
            btnExcluir.Text = "Excluir";
            btnExcluir.Click += configFormulario.ExcluirClick;
            btnExcluir.Attributes.Add("class", "botaoPrimaryPadrao");
            btnExcluir.Attributes.Add("Title", "Excluir");
            panel.Controls.Add(btnExcluir);
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
        panel.Controls.Add(new LiteralControl(@"</fieldset>")); //Fieldset - Fim

        return panel;
    }

    public Panel MontarUIFormularioExibir(List<CompBase> ListComponentesUI, NautaModelFormulario configFormulario, DataRow dadosCarregados)
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
        panel.Controls.Add(new LiteralControl(@"    <h4><b>Formulário de Exibição</b></h4>"));
        panel.Controls.Add(new LiteralControl(@"    <hr/>"));
        panel.Controls.Add(MontarComponentes(ListComponentesUI, tipoExibicaoPanel.Exibir, dadosCarregados));
        panel.Controls.Add(new LiteralControl(@"    <div class=""container-fluid mt-3"">"));
        panel.Controls.Add(new LiteralControl(@"        <div class=""row mob-row-buttons"">"));
        panel.Controls.Add(new LiteralControl(@"            <div class=""col-lg-6 text-left"">"));

        if (!configFormulario.ocultarBotaoCancelarExibir)
        {
            Button btnCancelar = new Button();
            btnCancelar.ID = "btnCancelarExibir";
            btnCancelar.Text = "Cancelar";
            btnCancelar.Click += configFormulario.EditarClick_cancelar;
            btnCancelar.Attributes.Add("class", "botaoSecondPadrao");
            btnCancelar.Attributes.Add("Title", "Cancelar");
            panel.Controls.Add(btnCancelar);
        }

        panel.Controls.Add(new LiteralControl(@"            </div>"));
        panel.Controls.Add(new LiteralControl(@"        </div>")); //Row - Fim
        panel.Controls.Add(new LiteralControl(@"    </div>")); //Container - Fim
        panel.Controls.Add(new LiteralControl(@"</fieldset>")); //Container - Fim

        return panel;
    }


}
