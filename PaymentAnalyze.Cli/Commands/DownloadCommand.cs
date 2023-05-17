using MoneyForward.Client;
using PaymentAnalyze.Cli.Settings;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Commands;

public class DownloadCommand :  AsyncCommand<DownloadCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DownloadCommandSettings settings)
    {
        using var tokenUtil = new TokenUtil(false, TimeSpan.FromSeconds(settings.SeleniumTimeoutMs));
        var token = tokenUtil.Login(settings.MailAddress, settings.Password);

        var moneyForwardClient = new MoneyForwardClient(token);
        for (var dateOnly = settings.Start; dateOnly < settings.Start.AddMonths(settings.MonthCount); dateOnly = dateOnly.AddMonths(1))
        {
            var fileName = $"{dateOnly.ToString("yyyy-MM")}.csv";
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