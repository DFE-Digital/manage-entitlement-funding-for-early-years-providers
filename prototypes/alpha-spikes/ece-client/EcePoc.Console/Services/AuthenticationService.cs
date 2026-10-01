using System.Net.Http.Json;
using System.Text.Json;
using EcePoc.Console.Configuration;
using EcePoc.Console.Models;

using Microsoft.Extensions.Options;

namespace EcePoc.Console.Services;

public class AuthenticationService(HttpClient httpClient, IOptions<EceOptions> options)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly EceOptions _options = options.Value;

    public async Task<string> GetTokenAsync()
    {
        var url = "/oauth2/token";

        KeyValuePair<string, string>[] formData = CreateTokenRequestData();

        using (var content = new FormUrlEncodedContent(formData))
        {
            content.Headers.ContentType!.MediaType = "application/x-www-form-urlencoded";

            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var responsePayload = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            return responsePayload?["access_token"]?.ToString()
                   ?? throw new InvalidOperationException("JWT not found in login response");
        }
    }

    private KeyValuePair<string, string>[] CreateTokenRequestData()
    {
        return new[]
        {
            new KeyValuePair<string, string>("client_id", _options.Username),
            new KeyValuePair<string, string>("client_secret", _options.Password),
            new KeyValuePair<string, string>("scope", "check working_families two_year_offer"),
            new KeyValuePair<string, string>("grant_type", "")
        };
    }
}