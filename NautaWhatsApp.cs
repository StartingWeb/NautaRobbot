using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class NautaWhatsApp : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public NautaWhatsApp(string baseUrl = "http://localhost:7072")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl.TrimEnd('/');
    }

    // Método Async (recomendado se seu código ASP.NET permitir)
    public async Task<string> EnviarMensagemAsync(string numero, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número é obrigatório.", nameof(numero));
        if (string.IsNullOrWhiteSpace(mensagem))
            throw new ArgumentException("Mensagem é obrigatória.", nameof(mensagem));

        var url = $"{_baseUrl}/send-message";

        var json = $"{{\"number\":\"{numero}\",\"message\":\"{mensagem}\"}}";

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return responseBody;
        else
            throw new HttpRequestException($"Erro {response.StatusCode}: {responseBody}");
    }

    // Método Síncrono para usar onde async não é possível
    public string EnviarMensagem(string numero, string mensagem)
    {
        try
        {
            return EnviarMensagemAsync(numero, mensagem).GetAwaiter().GetResult();
        }
        catch (AggregateException aex)
        {
            throw aex.InnerException ?? aex;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
