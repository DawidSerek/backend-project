using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Infrastructure.Import;

public static class CsvReaderFactory
{
    public static CsvReader CreateDefault(Stream stream)
    {
        var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            DetectDelimiter = true,
            BadDataFound = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim,
        };
        return new CsvReader(reader, config);
    }
}
