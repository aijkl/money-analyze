using System.Net;

namespace MoneyForward.Client;

public class ContentTypeException : Exception
{
    public ContentTypeException(string message) : base(message)
    {
    }
}

public class MoneyForwardClient : IDisposable
{
    private const string Url = "https://moneyforward.com/";
    private readonly HttpClient _httpclient;
    public MoneyForwardClient(string token)
    {
        _httpclient = new HttpClient();
        _httpclient.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
        _httpclient.DefaultRequestHeaders.Add("Cookie", new Cookie("_moneybook_session", token).ToString());
    }
    public async Task<HttpResponseMessage> FetchHistoryCsvAsync(DateOnly startDate)
    {
        var response = await _httpclient.GetAsync($"{Url}/cf/csv?from={startDate.ToString("yyyy/MM/dd/")}");
        var contentType = response.Headers.FirstOrDefault(x => x.Key == "Content-Type").Value?.ToList();
        if (contentType == null || contentType.All(x => x != "text/csv; charset=utf-8"))
        {
            throw new ContentTypeException("Content-TypeがCSVではありません、トークンの有効期限が切れている可能性があります");
        }
        return response;
    }
    public void Dispose()
    {
        _httpclient.Dispose();
    }
}