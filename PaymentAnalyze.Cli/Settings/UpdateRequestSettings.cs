using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Settings;

public class UpdateRequestSettings : SettingsBase
{
    public UpdateRequestSettings(string mailAddress, string password, string cacheFilePath) : base(mailAddress, password, cacheFilePath)
    {
    }

    [CommandOption("--interval-ms||-i")]
    public int IntervalMs { set; get; } = 1000;
}