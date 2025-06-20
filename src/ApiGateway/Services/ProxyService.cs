using System.Text;
using System.Text.Json;

namespace TrueCodeTestTask.ApiGateway.Services;

public class ProxyService(HttpClient httpClient, ILogger<ProxyService> logger)
{

    public async Task<HttpResponseMessage> ForwardRequestAsync(
        string targetUrl,
        HttpMethod method,
        string? content = null,
        string? authToken = null,
        string? contentType = "application/json")
    {
        try
        {
            var request = new HttpRequestMessage(method, targetUrl);

            // Add content if provided
            if (!string.IsNullOrEmpty(content))
            {
                request.Content = new StringContent(content, Encoding.UTF8, contentType);
            }

            // Add authorization header if provided
            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            }

            logger.LogInformation("Forwarding {Method} request to {Url}", method, targetUrl);

            var response = await httpClient.SendAsync(request);

            logger.LogInformation("Received response {StatusCode} from {Url}", response.StatusCode, targetUrl);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding request to {Url}", targetUrl);
            throw;
        }
    }

    public async Task<T?> ForwardRequestAsync<T>(
        string targetUrl,
        HttpMethod method,
        object? requestBody = null,
        string? authToken = null)
    {
        try
        {
            string? content = null;
            if (requestBody != null)
            {
                content = JsonSerializer.Serialize(requestBody);
            }

            var response = await ForwardRequestAsync(targetUrl, method, content, authToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return default;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error forwarding request to {Url}", targetUrl);
            return default;
        }
    }
}
