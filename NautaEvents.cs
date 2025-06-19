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
        if (_eventosFormulario.PesquisarClick_cancelar == null)
            _eventosFormulario.PesquisarClick_cancelar = eventPesquisarCancelarClick;

        if (_eventosFormulario.EditarClick == null)
            _eventosFormulario.EditarClick = eventEdicaoClick;

        if (_eventosFormulario.AdicionarClick_cancelar == null)
            _eventosFormulario.AdicionarClick_cancelar = eventAdicionaCancelarClick;
    }


    public void eventPesquisarCancelarClick(object sender, EventArgs e)
    {
        _nautaHelper.ReloadPage();
    }

    public void eventPesquisarClick(object sender, EventArgs e)
    {

    }

    public void eventEdicaoCancelarClick(object sender, EventArgs e)
    {
        string url = "" + HttpContext.Current.Request.Url.AbsolutePath;
        HttpContext.Current.Response.Redirect(url);
    }

    public void eventEdicaoClick(object sender, EventArgs e)
    {
        _nautaInstance.debugEventosInternos = _nautaInstance.EditarDados();
        HttpContext.Current.Session["NautaDebugInterno"] = _nautaInstance.debugEventosInternos;
        _nautaHelper.ReloadPage();
    }

    public void eventAdicionarClick(object sender, EventArgs e)
    {
    }

    public void eventAdicionaCancelarClick(object sender, EventArgs e)
    {
    }


    public NautaModelFormulario RetornaEventosFormularioValidados()
    {
        return _eventosFormulario;
    }
}

