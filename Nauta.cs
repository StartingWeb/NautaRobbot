using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NautaRobbot;
using NautaRobbot.Helpers;
using static NautaRepository;


public class Nauta
{
    private NautaModel _nautaModel { get; set; }
    private NautaRepository _repository;
    private readonly NautaHelper _helper;

    public List<CompBase> Componentes = new List<CompBase>();
    public NautaModelSQL nautaSQL;
    public NautaModelFormulario configFormulario;

    bool modoInserir = false;
    bool modoEditar = false;
    bool modoPesquisar = false;
    bool modoExibir = false;

    public NautaBuild debugEventosInternos;

    public Nauta(NautaModel model, string modo)
    {
        _nautaModel = model;
        _repository = new NautaRepository();
        _helper = new NautaHelper();

        VerificarDebugInternoPosPost();
        DefinirPanelExibicao(modo);
    }

    void VerificarDebugInternoPosPost()
    {
        debugEventosInternos = (NautaBuild)HttpContext.Current.Session["NautaDebugInterno"];
        HttpContext.Current.Session.Remove("NautaDebugInterno");  // limpa para não repetir mensagem

        if (debugEventosInternos == null)
            debugEventosInternos = new NautaBuild();
    }
    void DefinirPanelExibicao(string modo)
    {
        switch (modo)
        {
            case "p":
                modoPesquisar = true;
                break;
            case "i":
                modoInserir = true;
                break;
            case "e":
                modoEditar = true;
                break;
            case "v":
                modoExibir = true;
                break;
            case "":
                modoPesquisar = true;
                break;
        }

        _nautaModel.panelPesquisar.Visible = modoPesquisar;
        _nautaModel.panelListagem.Visible = modoPesquisar;
        _nautaModel.panelInserir.Visible = modoInserir;
        _nautaModel.panelEditar.Visible = modoEditar;
        _nautaModel.panelExibir.Visible = modoExibir;
    }

    DataRow RetornaLinhaRodapeListagem(Dictionary<string, decimal> dicionarioRodape, DataTable novaTabela)
    {
        DataRow linhaRodape = novaTabela.NewRow();
        linhaRodape["Contagem"] = "Total:";
        linhaRodape["Actions"] = "";

        foreach (DataColumn coluna in novaTabela.Columns)
        {
            object valorRodape = DBNull.Value;
            if (dicionarioRodape.ContainsKey(coluna.ColumnName))
                valorRodape = dicionarioRodape[coluna.ColumnName];

            linhaRodape[coluna.ColumnName] = valorRodape;
        }

        return linhaRodape;
    }

    DataTable ReconstruirColunasDataTableListagem(DataTable dadosListagem)
    {
        DataTable tabelaNova = new DataTable();
        int numeroLinha = 0;

        tabelaNova.Columns.Add("Contagem", typeof(string));
        tabelaNova.Columns.Add("Actions", typeof(string));

        foreach (DataColumn coluna in dadosListagem.Columns)
        {
            tabelaNova.Columns.Add(coluna.ColumnName, coluna.DataType);
        }

        Dictionary<string, decimal> dicionarioRodape = new Dictionary<string, decimal>();

        foreach (DataRow linha in dadosListagem.Rows)
        {
            numeroLinha++;
            DataRow novaLinha = tabelaNova.NewRow();

            int idRegistro = Fac.convertInt(linha[nautaSQL.primaryKey].ToString());
            string botoesDeAcao = _helper.RetornaBotoesListagemPadrao(idRegistro,
                configFormulario.ocultarBotaoEditarListagem,
                configFormulario.ocultarBotaoExibirListagem);

            novaLinha["Contagem"] = numeroLinha;
            novaLinha["Actions"] = botoesDeAcao;

            foreach (DataColumn coluna in dadosListagem.Columns)
            {
                novaLinha[coluna.ColumnName] = linha[coluna.ColumnName]; //Colocando todas as colunas na linha nova.

                // Somar o valor se for uma coluna numérica
                if (decimal.TryParse(linha[coluna.ColumnName].ToString(), out decimal valorCelula))
                {
                    if (dicionarioRodape.ContainsKey(coluna.ColumnName))
                        valorCelula += dicionarioRodape[coluna.ColumnName];

                    dicionarioRodape[coluna.ColumnName] = valorCelula;
                }
            }

            tabelaNova.Rows.Add(novaLinha);
        }

        if (dicionarioRodape.Count > 0)
            tabelaNova.Rows.Add(RetornaLinhaRodapeListagem(dicionarioRodape, tabelaNova));

        return tabelaNova;
    }


    //Public - acessiveis
    public NautaBuild MontarFormularios()
    {
        NautaBuild build = new NautaBuild();
        configFormulario = new NautaEvents(configFormulario, this).RetornaEventosFormularioValidados();

        if (modoPesquisar) MontarFormularioPesquisar();
        else if (modoInserir) MontarFormularioAdicionar();
        else if (modoEditar) MontarFormularioEditar();
        else if (modoExibir) MontarFormularioExibir();

        return build;
    }


    //Ações de interação DataBase
    public NautaBuild PesquisarDados()
    {
        NautaBuild debug = new NautaBuild();
        NautaRepository repositoryPesquisa = new NautaRepository();
        foreach (var componente in Componentes)
        {

            string valorCampo = _helper.RecuperaValorComponentePanel(_nautaModel.panelEditar, componente) ?? "";
            bool valorMonetario = false;

            if (componente.GetType() == typeof(CompTextBox))
            {
                CompTextBox textBox = (CompTextBox)componente;
                valorMonetario = textBox.ValorMonetario;
            }

            repositoryPesquisa.camposSQL.Add(new CampoSQL
            {
                chave = componente.SQL.campoSQL,
                valor = valorCampo,
                valorMonetario = valorMonetario,
                tipoComponente = componente.GetType()
            });

        }

        DataTable dadosPesquisa = repositoryPesquisa.RetornaDadosPesquisa(nautaSQL);
        DataTable dadosPesquisaTratado = ReconstruirColunasDataTableListagem(dadosPesquisa);
        debug = MontarListagemResultadoPesquisa(dadosPesquisaTratado);
        return debug;
    }

    public NautaBuild AdicionarDados()
    {
        NautaBuild debug = new NautaBuild();
        NautaRepository repositoryAdicao = new NautaRepository();
        string mensagemErroObrigatorio = "";

        try
        {
            foreach (var componente in Componentes)
            {
                string valorComponente = _helper.RecuperaValorComponentePanel(_nautaModel.panelEditar, componente) ?? "";

                if (componente.SQL.acaoSQLInsert)
                {
                    if (valorComponente.Equals("") && !componente.SQL.valorPadrao.Equals(""))
                        valorComponente = componente.SQL.valorPadrao;

                    if (!valorComponente.Equals(""))
                    {
                        repositoryAdicao.camposSQL.Add(new CampoSQL
                        {
                            chave = componente.SQL.campoSQL,
                            valor = valorComponente,
                            ehTexto = componente.SQL.valorPadrao.Equals("") //Se tiver valor padrão é comando SQL
                        });
                    }
                    else if (valorComponente.Equals("") && componente.Config.campoObrigatorio)
                    {
                        mensagemErroObrigatorio += "<b>" + componente.HTML.label + "</b> <br/>";
                    }
                }
            }

            if (!mensagemErroObrigatorio.Equals(""))
            {
                mensagemErroObrigatorio = "Os seguintes campos são obrigatórios serem preenchidos: <br/>" + mensagemErroObrigatorio;
                debug.Sucesso = false;
                debug.Mensagem = mensagemErroObrigatorio;
            }
            else if (repositoryAdicao.camposSQL.Count > 0)
                debug = repositoryAdicao.AdicionarDados(nautaSQL);
            else
            {
                debug.Sucesso = false;
                debug.Mensagem = "Nenhum valor atualizado, nenhum componente listado!";
            }
        }
        catch (Exception ex)
        {
            debug.Sucesso = false;
            debug.RetornoDesenvolvimento = "Erro: " + ex.ToString();
            throw new Exception(debug.RetornoDesenvolvimento);
        }

        return debug;
    }

    public NautaBuild EditarDados()
    {
        NautaBuild debug = new NautaBuild();
        NautaRepository repositoryEdicao = new NautaRepository();
        string mensagemErroObrigatorio = "";

        try
        {
            foreach (var componente in Componentes)
            {
                string valorComponente = _helper.RecuperaValorComponentePanel(_nautaModel.panelEditar, componente) ?? "";

                if (valorComponente.Equals("") && !componente.SQL.valorPadrao.Equals(""))
                    valorComponente = componente.SQL.valorPadrao;

                if (componente.SQL.acaoSQLUpdate)
                {
                    if (!valorComponente.Equals(""))
                    {
                        repositoryEdicao.camposSQL.Add(new CampoSQL
                        {
                            chave = componente.SQL.campoSQL,
                            valor = valorComponente
                        });
                    }
                    else if (valorComponente.Equals("") && componente.Config.campoObrigatorio)
                    {
                        mensagemErroObrigatorio += "<b>" + componente.HTML.label + "</b> <br/>";
                    }
                }
            }

            if (!mensagemErroObrigatorio.Equals(""))
            {
                mensagemErroObrigatorio = "Os seguintes campos são obrigatórios serem preenchidos: <br/>" + mensagemErroObrigatorio;
                debug.Sucesso = false;
                debug.Mensagem = mensagemErroObrigatorio;
            }
            else if (repositoryEdicao.camposSQL.Count > 0)
                debug = repositoryEdicao.EditarDados(nautaSQL);
            else
            {
                debug.Sucesso = false;
                debug.Mensagem = "Nenhum valor atualizado, nenhum componente listado!";
            }
        }
        catch (Exception ex)
        {
            debug.Sucesso = false;
            debug.RetornoDesenvolvimento = "Erro: " + ex.ToString();
            throw new Exception(debug.RetornoDesenvolvimento);
        }
        return debug;
    }

    public NautaBuild ExcluirDados()
    {
        NautaRepository repositoryExclusao = new NautaRepository();
        return repositoryExclusao.ExcluirDados(nautaSQL);
    }




    //Private - Interno
    private NautaBuild MontarFormularioPesquisar()
    {
        NautaBuild debug = new NautaBuild();
        var uiBuilder = new NautaUiBuilder();
        uiBuilder.isPostBack = _nautaModel.isPostBack;

        try
        {
            var painel = uiBuilder.MontarUIFormularioPesquisa(Componentes, configFormulario);
            _nautaModel.panelPesquisar.Controls.Clear();
            _nautaModel.panelPesquisar.Controls.Add(painel);

            debug.Sucesso = true;
        }
        catch (Exception ex)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Ops, houve um erro ao criar esse formulario, entre em contato " +
                "com a equipe de desenvolvimento";
            debug.RetornoDesenvolvimento = ex.ToString();
        }
        return debug;
    }

    private NautaBuild MontarListagemResultadoPesquisa(DataTable dadosPesquisa)
    {
        NautaBuild debug = new NautaBuild();
        var uiBuilder = new NautaUiBuilder();
        uiBuilder.isPostBack = _nautaModel.isPostBack;
        try
        {
            var painel = uiBuilder.MontarUIListagem(Componentes, dadosPesquisa);
            _nautaModel.panelListagem.Visible = true;
            _nautaModel.panelListagem.Controls.Clear();
            _nautaModel.panelListagem.Controls.Add(painel);
            debug.Sucesso = true;
        }
        catch (Exception ex)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Ops, houve um erro ao criar essa listagem, entre em contato " +
                "com a equipe de desenvolvimento";
            debug.RetornoDesenvolvimento = ex.ToString();
        }
        return debug;
    }

    private NautaBuild MontarFormularioAdicionar()
    {
        NautaBuild debug = new NautaBuild();
        var uiBuilder = new NautaUiBuilder();
        uiBuilder.isPostBack = _nautaModel.isPostBack;

        try
        {
            var painel = uiBuilder.MontarUIFormularioAdicionar(Componentes, configFormulario);
            _nautaModel.panelInserir.Controls.Clear();
            _nautaModel.panelInserir.Controls.Add(painel);

            debug.Sucesso = true;
        }
        catch (Exception ex)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Ops, houve um erro ao criar esse formulario, entre em contato " +
                "com a equipe de desenvolvimento";
            debug.RetornoDesenvolvimento = ex.ToString();
        }

        return debug;
    }

    private NautaBuild MontarFormularioEditar()
    {
        NautaBuild build = new NautaBuild();
        DataRow dataEdicao = _repository.RetornaDadosFormularioEdicao(nautaSQL);
        if (dataEdicao == null)
        {
            build.Sucesso = false;
            build.Mensagem = "Nenhuma informação encontrada para esse registro!";
            build.RedirecionamentoInicial = true;
        }
        else
        {
            var uiBuilder = new NautaUiBuilder();
            uiBuilder.isPostBack = _nautaModel.isPostBack;
            var painel = uiBuilder.MontarUIFormularioEdicao(Componentes, dataEdicao, configFormulario);

            _nautaModel.panelEditar.Controls.Clear(); // sempre bom limpar antes
            _nautaModel.panelEditar.Controls.Add(painel);

            build.Sucesso = true;

        }

        return build;
    }

    private NautaBuild MontarFormularioExibir()
    {
        NautaBuild debug = new NautaBuild();
        DataRow dataExibicao = _repository.RetornaDadosFormularioExibicao(nautaSQL);

        var uiBuilder = new NautaUiBuilder();
        uiBuilder.isPostBack = _nautaModel.isPostBack;
        try
        {
            var painel = uiBuilder.MontarUIFormularioExibir(Componentes, configFormulario, dataExibicao);
            _nautaModel.panelExibir.Controls.Clear();
            _nautaModel.panelExibir.Controls.Add(painel);

            debug.Sucesso = true;
        }
        catch (Exception ex)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Ops, houve um erro ao criar esse formulario, entre em contato " +
                "com a equipe de desenvolvimento";
            debug.RetornoDesenvolvimento = ex.ToString();
        }
        return debug;
    }

}

