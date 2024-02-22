using MoneyForward.Client;
using PaymentAnalyze.Cli.Settings;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Commands;

public class DownloadCommand :  AsyncCommand<DownloadCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DownloadCommandSettings settings)
    {
        var cacheData = CacheData.LoadFromFile(settings.CacheFilePath);
        if (DateTime.Now - cacheData.LastLogin > TimeSpan.FromDays(20))
        {
            using var tokenUtil = new TokenUtil(true, TimeSpan.FromSeconds(settings.SeleniumTimeoutMs));
            var timestamp = DateTime.Now;
            cacheData.Token = tokenUtil.Login(settings.MailAddress, settings.Password);
            cacheData.LastLogin = timestamp;
            cacheData.SaveToFile();
        }

        var moneyForwardClient = new MoneyForwardClient(cacheData.Token);
        for (var dateOnly = settings.Start; dateOnly < settings.Start.AddMonths(settings.MonthCount); dateOnly = dateOnly.AddMonths(1))
        {
            var fileName = $"{dateOnly:yyyy-MM}.csv";
            using var response = await moneyForwardClient.FetchHistoryCsvAsync(dateOnly);
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileSteam = new FileStream(fileName, FileMode.OpenOrCreate);
            await stream.CopyToAsync(fileSteam);
            
            AnsiConsoleHelper.MarkupLine($"Download {fileName}", AnsiConsoleHelper.State.Success);
            await Task.Delay(settings.IntervalMs);
        }
        return 0;
    }
}