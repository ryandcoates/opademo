using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class OpaAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;
    private readonly string _opaUrl;

    public OpaAuthorizationMiddleware(RequestDelegate next, string opaUrl)
    {
        _next = next;
        _httpClient = new HttpClient();
        _opaUrl = opaUrl;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Split the path into segments
        var pathSegments = context.Request.Path.Value?.Trim('/').Split('/');

        var input = new
        {
            method = context.Request.Method,
            path = pathSegments,
            user = context.User?.Identity?.Name ?? "anonymous",
        };

        var opaRequest = new
        {
            input = input
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(opaRequest), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_opaUrl, requestContent);

        if (!response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthorizationResult>(responseBody);

        // Check if the request is authorized
        if (result?.IsAuthorized == true)
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }
    }


}

public class AuthorizationResult
{
    [JsonPropertyName("result")]
    public bool IsAuthorized { get; set; }
}
