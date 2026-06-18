using ApplicationCore.Interfaces.Validation;

namespace Infrastructure.Validation;

public class KrsValidator(HttpClient http) : IKrsValidator
{
    public async Task<bool> ValidateKrsAsync(string krs)
    {
        try
        {
            var response = await http.GetAsync($"https://api-krs.ms.gov.pl/api/krs/{krs}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException) { return false; }
        catch (TaskCanceledException) { return false; }
    }
}
