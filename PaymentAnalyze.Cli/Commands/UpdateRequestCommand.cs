using MoneyForward.Client;
using PaymentAnalyze.Cli.Settings;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Commands;

public class UpdateRequestCommand : CommandBase<UpdateRequestSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, UpdateRequestSettings settings)
    {
        var cacheData = CacheData.LoadFromFile(settings.CacheFilePath);
        LoginIfNeed(cacheData, settings);
        
        var moneyForwardClient = new MoneyForwardClient(cacheData.Token);
        var csrfToken = await moneyForwardClient.FetchCsrfTokenAsync();
        await moneyForwardClient.SendUpdateRequestAsync(csrfToken);
            
        AnsiConsoleHelper.MarkupLine($"Update Request", AnsiConsoleHelper.State.Success);
        return 0;
    }
}