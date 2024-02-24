using Spectre.Console;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Settings;

public class DownloadCommandSettings : SettingsBase
{
    public DownloadCommandSettings(string mailAddress, string password, string cacheFilePath, string csvDirectory) : base(mailAddress, password, cacheFilePath)
    {
        MailAddress = mailAddress;
        Password = password;
        CsvDirectory = csvDirectory;
    }
    
    [CommandOption("--download-interval-ms")]
    public int IntervalMs { set; get; } = 3000;

    [CommandOption("--start")]
    public DateOnly Start { set; get; }

    [CommandOption("--month-count")] 
    public int MonthCount { set; get; } = 1;

    [CommandOption("--csv-directory")] 
    public string CsvDirectory { set; get; }

    public override ValidationResult Validate()
    {
        return Start == DateOnly.MinValue ? ValidationResult.Error("日付が正しくありません") : base.Validate();
    }
}