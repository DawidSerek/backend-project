using ApplicationCore.Interfaces.Validation;

namespace Infrastructure.Validation;

public class WebsiteValidator(HttpClient http) : IWebsiteValidator
{
    public async Task<bool> ValidateAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return false;
        }

        try
        {
            var response = await http.GetAsync(uri);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException) { return false; }
        catch (TaskCanceledException) { return false; }
    }
}
