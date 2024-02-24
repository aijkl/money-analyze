using System.Net;
using System.Text.RegularExpressions;

namespace MoneyForward.Client;

public class ContentTypeException : Exception
{
    public ContentTypeException(string message) : base(message)
    {
    }
}

public partial class MoneyForwardClient : IDisposable
{
    private const string Url = "https://moneyforward.com";
    private readonly HttpClient _httpClient;
    public MoneyForwardClient(string token)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Cookie", new Cookie("_moneybook_session", token).ToString());
    }
    public async Task<string> FetchCsrfTokenAsync()
    {
        var response = await _httpClient.GetAsync($"{Url}/");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var match = CsrfTokenRegex().Match(html);
        return match.Groups[1].Value;
    }
    public async Task SendUpdateRequestAsync(string csrfToken)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, $"{Url}/aggregation_queue");
        httpRequestMessage.Headers.Add("X-CSRF-Token", csrfToken);
        httpRequestMessage.Headers.Add("X-Requested-With", "XMLHttpRequest");
        
        var response = await _httpClient.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
    }
    public async Task<HttpResponseMessage> FetchHistoryCsvAsync(DateOnly startDate)
    {
        var response = await _httpClient.GetAsync($"{Url}/cf/csv?from={startDate:yyyy/MM/dd}");
        var contentType = response.Content.Headers.FirstOrDefault(x => x.Key == "Content-Type").Value?.ToList();
        response.EnsureSuccessStatusCode();
        if (contentType == null || contentType.All(x => x != "text/csv; charset=utf-8"))
        {
            throw new ContentTypeException("Content-TypeがCSVではありません、トークンの有効期限が切れている可能性があります");
        }
        return response;
    }
    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [GeneratedRegex(@"<meta\s+name=""csrf-token""\s+content=""([^""]+)""\s*\/?>")]
    private static partial Regex CsrfTokenRegex();
}