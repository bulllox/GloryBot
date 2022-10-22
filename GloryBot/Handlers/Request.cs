using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace GloryBot.Handlers;
public class Request
{
    private HttpClient client { get; set; } = new();
    private string Url { get; set; } = "";
    private string Method { get; set; } = "Get";
    private string Content { get; set; } = "";
    private string ContentType { get; set; } = "";
    private Dictionary<string, dynamic> headers { get; set; } = new();

    public Request(string url, string method)
    {
        this.Url = url;
        this.Method = method;
        this.headers = new();
        client = new();
    }

    public void SetHeader(Dictionary<string, dynamic> header)
    {
        headers = header;
    }

    public void SetContent(string content)
    {
        this.Content = content;
    }

    public void SetContent(Dictionary<string, dynamic> content)
    {
        this.Content = JsonConvert.SerializeObject(content, Formatting.Indented);
    }

    public void SetContentType(string contentType)
    {
        this.ContentType = contentType;
    }

    public async Task<HttpResponseMessage> Execute()
    {
        if (!string.IsNullOrEmpty(this.Method) && !string.IsNullOrEmpty(this.Url))
        {
            using (var request = new HttpRequestMessage(new HttpMethod(this.Method), this.Url))
            {
                if (this.headers != null)
                {
                    foreach (var key in this.headers.Keys)
                    {
                        request.Headers.TryAddWithoutValidation(key, this.headers[key]);
                    }
                }
                var res = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(this.Content);
                if (!string.IsNullOrEmpty(this.Content)) request.Content = new StringContent(JsonConvert.SerializeObject(res, Formatting.Indented));
                if (request.Content != null && !string.IsNullOrEmpty(this.ContentType)) request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(this.ContentType);

                return await this.client.SendAsync(request);
            }
        }
        else
        {
            return null;
        }
    }

}
