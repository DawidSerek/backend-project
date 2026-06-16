using System.Text.Json;

namespace Infrastructure.Import;

public static class JsonOptionsFactory
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
