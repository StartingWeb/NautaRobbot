using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using NautaRobbot.Helpers;
using NautaRobbot;

class NautaTeste
{
    //Simulador de frontEnd - INI
    string modo = "e";
    int idClient = 1;
    bool IsPostBack = false;
    Panel pnlPesquisa = new Panel();
    Panel pnlInserir = new Panel();
    Panel pnlEditar = new Panel();
    Panel pnlListagem = new Panel();
    Panel pnlExibir = new Panel();
    Panel pnlGridFilho = new Panel();
    //Simulador de frontEnd - FIM


    NautaHelper _helper = new NautaHelper();


    void Page_Load(object sender, EventArgs e)
    {
        Nauta nautaRobbot = new Nauta(new NautaModel
        {
            panelPesquisar = pnlPesquisa,
            panelInserir = pnlInserir,
            panelEditar = pnlEditar,
            panelListagem = pnlListagem,
            panelExibir = pnlExibir,
            panelGridFilho = pnlGridFilho,
            isPostBack = IsPostBack
        }, modo);

        NautaModelFormulario configFormulario = ConfigurarFormulariosPage();
        nautaRobbot.nautaSQL = ConfigurarSQLPage();
        MontarComponentesUI(nautaRobbot);
        NautaBuild buildPage = nautaRobbot.MontarFormularios(configFormulario);
        ExibirRetornoNauta(buildPage, nautaRobbot.debugEventosInternos);
    }

    void ExibirRetornoNauta(NautaBuild buildFormulario, NautaBuild buildEvents)
    {
        string mensagemExibir = buildEvents?.Mensagem ?? buildFormulario.Mensagem;
        bool sucessoOperacao = buildEvents?.Sucesso ?? buildFormulario.Sucesso;

        if (!string.IsNullOrEmpty(mensagemExibir))
        {
            if (sucessoOperacao)
            {
                //Exibe mensagem do tipo sucesso
            }
            else
            {
                //exibe mensagem do tipo erro
            }
        }

    }

    void MontarComponentesUI(Nauta nautaRobbot)
    {
        nautaRobbot.Componentes.Add(new CompTextBox
        {
            HTML = new CompBase_HTML { label = "Nome" },
            SQL = new CompBase_SQL
            {
                campoSQL = "chr_usuario",
                acaoSQLInsert = true,
                acaoSQLUpdate = true,
                valorPadrao = "",
            },
            Config = new CompBase_Config { campoObrigatorio = true },
            Modulos = new CompBase_Modulos
            {
                Pesquisar = true,
                Editar = true,
                Exibir = true,
                Inserir = true,
                Listagem = true
            }
        });
    }

    NautaModelFormulario ConfigurarFormulariosPage()
    {
        var configFormulario = new NautaModelFormulario
        {
            tituloPrincipal = "Usuarios",
            subTitulo = "Usuarios da base de teste",

            //Eventos
            AdicionarClick = eventAdicionarCancelar,

            PesquisarClick = eventPesquisar,
            PesquisarClick_cancelar = eventPesquisarCancelar,

        };

        return configFormulario;
    }

    NautaModelSQL ConfigurarSQLPage()
    {
        var configSQL = new NautaModelSQL
        {
            idClient = idClient,
            primaryKey = "id_usuario",
            selectorFrom = "*",
            table = "AuthUsuario",
            where = "",
            orderBy = "id_usuario Desc",
            locationSelect = "AuthUsuario",
            locationInsert = "AuthUsuario",
            locationUpdate = "AuthUsuario",
            locationDelete = "AuthUsuario"
        };

        return configSQL;
    }


    void eventPesquisarCancelar(object sender, EventArgs e)
    {
        //Recarrega a página
        //_helper.reload();
    }

    void eventPesquisar(object sender, EventArgs e)
    {
        //stw.montarlistagem();
    }

    void eventAdicionarCancelar(object sender, EventArgs e)
    {
        //Response.Redirect("/_System/Cultos.aspx");
    }

}

