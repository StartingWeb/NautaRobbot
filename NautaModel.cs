using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using NautaRobbot;

public class NautaModel
{
    //Paineis
    public Panel panelPesquisar;
    public Panel panelInserir;
    public Panel panelEditar;
    public Panel panelListagem;
    public Panel panelExibir;
    public Panel panelGridFilho;

    public bool isPostBack = false;
}

public class NautaModelFormulario
{
    //Configuração Inicial
    public string tituloPrincipal { get; set; }
    public string subTitulo { get; set; }

    //Exibições
    public bool ocultarBotaoAdicionar { get; set; } = false;
    public bool ocultarBotaoCancelarEditar { get; set; } = false;
    public bool ocultarBotaoExcluirEditar { get; set; } = false;
    public bool ocultarBotaoExcluirrExibir { get; set; } = false;
    public bool ocultarBotaoCancelarExibir { get; set; } = false;
    public bool ocultarBotaoCancelarAdicionar { get; set; } = false;

    public bool ocultarBotaoEditarListagem { get; set; } = false;
    public bool ocultarBotaoExibirListagem { get; set; } = false;

    //Eventos
    public EventHandler PesquisarClick;
    public EventHandler PesquisarClick_cancelar;

    public EventHandler AdicionarClick;
    public EventHandler AdicionarClick_cancelar;

    public EventHandler EditarClick;
    public EventHandler EditarClick_cancelar;

    public EventHandler ExibirClick;
    public EventHandler ExibirClick_cancelar;

    public EventHandler ExcluirClick;

}


public class NautaModelSQL
{
    /// <summary>
    /// Id que faz o request do client
    /// </summary>
    public int idClient { get; set; }
    public string primaryKey { get; set; }

    /// <summary>
    /// Geralmente uso * -> pra trazer tudo
    /// </summary>
    public string selectorFrom { get; set; }
    public string table { get; set; }
    public string where { get; set; }
    public string orderBy { get; set; }
    public string locationSelect { get; set; }
    public string locationInsert { get; set; }
    public string locationUpdate { get; set; }
    public string locationDelete { get; set; }
}
