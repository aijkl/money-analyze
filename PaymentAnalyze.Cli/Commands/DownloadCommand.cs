using MoneyForward.Client;
using PaymentAnalyze.Cli.Settings;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Commands;

public class DownloadCommand :  CommandBase<DownloadCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DownloadCommandSettings settings)
    {
        var cacheData = CacheData.LoadFromFile(settings.CacheFilePath);
        LoginIfNeed(cacheData, settings);
        Directory.CreateDirectory(settings.CsvDirectory);
        
        var moneyForwardClient = new MoneyForwardClient(cacheData.Token);
        for (var dateOnly = settings.Start; dateOnly < settings.Start.AddMonths(settings.MonthCount); dateOnly = dateOnly.AddMonths(1))
        {
            var filePath = Path.Combine(settings.CsvDirectory, $"{dateOnly:yyyy-MM}.csv");
            using var response = await moneyForwardClient.FetchHistoryCsvAsync(dateOnly);
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileSteam = new FileStream(filePath, FileMode.OpenOrCreate);
            await stream.CopyToAsync(fileSteam);
            
            AnsiConsoleHelper.MarkupLine($"Download {filePath}", AnsiConsoleHelper.State.Success);
            await Task.Delay(settings.IntervalMs);
        }
        return 0;
    }
}