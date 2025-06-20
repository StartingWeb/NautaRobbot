using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class NautaEvents
{

    private NautaModelFormulario _eventosFormulario;
    private readonly Nauta _nautaInstance;
    private readonly NautaHelper _nautaHelper;

    public NautaEvents(NautaModelFormulario eventosFormulario, Nauta nauta)
    {
        _eventosFormulario = eventosFormulario;
        _nautaInstance = nauta;
        _nautaHelper = new NautaHelper();
        ValidarEventos(eventosFormulario);
    }

    void ValidarEventos(NautaModelFormulario eventosFormulario)
    {
        //Pesquisar
        if (_eventosFormulario.PesquisarClick == null)
            _eventosFormulario.PesquisarClick = eventPesquisarClick;

        if (_eventosFormulario.PesquisarClick_cancelar == null)
            _eventosFormulario.PesquisarClick_cancelar = eventPesquisarCancelarClick;

        //Adicionar
        if (_eventosFormulario.AdicionarClick == null)
            _eventosFormulario.AdicionarClick = eventAdicionarClick;

        if (_eventosFormulario.AdicionarClick_cancelar == null)
            _eventosFormulario.AdicionarClick_cancelar = eventAdicionarCancelarClick;


        //Editar
        if (_eventosFormulario.EditarClick == null)
            _eventosFormulario.EditarClick = eventEdicaoClick;

        if (_eventosFormulario.EditarClick_cancelar == null)
            _eventosFormulario.EditarClick_cancelar = eventEdicaoCancelarClick;

        //Exibir
        if (_eventosFormulario.ExibirClick_cancelar == null)
            _eventosFormulario.ExibirClick_cancelar = eventExibirCancelarClick;
    }


    //Pesquisar
    public void eventPesquisarClick(object sender, EventArgs e)
    {
        _nautaInstance.debugEventosInternos = _nautaInstance.PesquisarDados();
        HttpContext.Current.Session["NautaDebugInterno"] = _nautaInstance.debugEventosInternos;
    }

    public void eventPesquisarCancelarClick(object sender, EventArgs e)
    {
        _nautaHelper.ReloadPage();
    }


    //Adicionar
    public void eventAdicionarClick(object sender, EventArgs e)
    {
        _nautaInstance.debugEventosInternos = _nautaInstance.AdicionarDados();
        HttpContext.Current.Session["NautaDebugInterno"] = _nautaInstance.debugEventosInternos;

        if (_nautaInstance.debugEventosInternos.Sucesso)
            _nautaHelper.RedirectFormularioInicial();
        else
            _nautaHelper.ReloadPage();
    }

    public void eventAdicionarCancelarClick(object sender, EventArgs e)
    {
        _nautaHelper.RedirectFormularioInicial();
    }




    //Editar
    public void eventEdicaoClick(object sender, EventArgs e)
    {
        _nautaInstance.debugEventosInternos = _nautaInstance.EditarDados();
        HttpContext.Current.Session["NautaDebugInterno"] = _nautaInstance.debugEventosInternos;
        if (_nautaInstance.debugEventosInternos.Sucesso)
            _nautaHelper.RedirectFormularioInicial();
        else
            _nautaHelper.ReloadPage();
    }

    public void eventEdicaoCancelarClick(object sender, EventArgs e)
    {
        _nautaHelper.RedirectFormularioInicial();
    }



    //Exibir
    public void eventExibirCancelarClick(object sender, EventArgs e)
    {
        _nautaHelper.RedirectFormularioInicial();
    }






    public NautaModelFormulario RetornaEventosFormularioValidados()
    {
        return _eventosFormulario;
    }
}

