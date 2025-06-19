using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NautaRobbot;


public class Nauta
{
    private NautaModel _nautaModel { get; set; }
    private NautaRepository _repository;
    private readonly NautaHelper _helper;

    public List<CompBase> Componentes = new List<CompBase>();
    public NautaModelSQL nautaSQL;

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


    //Public - acessiveis
    public NautaBuild MontarFormularios(NautaModelFormulario configFormulario)
    {
        NautaBuild build = new NautaBuild();
        configFormulario = new NautaEvents(configFormulario, this).RetornaEventosFormularioValidados();

        if (modoPesquisar) MontarFormularioPesquisar(configFormulario);
        else if (modoEditar) MontarFormularioEdicao(configFormulario);

        return build;
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
                CompBase componenteBase = (CompBase)componente;
                string valorCampo = _helper.RecuperaValorComponentePanel(_nautaModel.panelEditar, componente) ?? "";

                if (componente.GetType() == typeof(CompTextBox))
                {
                    CompTextBox textBox = (CompTextBox)componente;
                }

                if (!valorCampo.Equals(""))
                {
                    repositoryEdicao.camposTransactSQLs.Add(new NautaRepository.CamposTransactSQL
                    {
                        chave = componenteBase.SQL.campoSQL,
                        valor = valorCampo
                    });
                }
                else if (valorCampo.Equals("") && componenteBase.Config.campoObrigatorio)
                {
                    mensagemErroObrigatorio += "<b>" + componenteBase.HTML.label + "</b> <br/>";
                }
            }

            if (!mensagemErroObrigatorio.Equals(""))
            {
                mensagemErroObrigatorio = "Os seguintes campos são obrigatórios serem preenchidos: <br/>" + mensagemErroObrigatorio;
                debug.Sucesso = false;
                debug.Mensagem = mensagemErroObrigatorio;
            }
            else if (repositoryEdicao.camposTransactSQLs.Count > 0)
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


    //Private - Interno
    private NautaBuild MontarFormularioPesquisar(NautaModelFormulario configFormulario)
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
        catch(Exception ex)
        {
            debug.Sucesso = false;
            debug.Mensagem = "Ops, houve um erro ao criar esse formulario, entre em contato " +
                "com a equipe de desenvolvimento";
            debug.RetornoDesenvolvimento = ex.ToString();
        }
        return debug;
    }

    private NautaBuild MontarFormularioEdicao(NautaModelFormulario configFormulario)
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

    
}

