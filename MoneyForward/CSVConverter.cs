using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace MoneyForward;

public static class CsvConverter
{
    public static IEnumerable<Transaction> Convert(string path, Encoding encoding)
    {
        using var textReader = new StreamReader(path, encoding);
        using var csvReader = new CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim
        });
        csvReader.Context.RegisterClassMap<ExpensesMapper>();
        return csvReader.GetRecords<Transaction>().ToList();
    }
}