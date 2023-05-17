using Spectre.Console;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Settings;

public class DownloadCommandSettings : CommandSettings
{
    public DownloadCommandSettings(string mailAddress, string password)
    {
        MailAddress = mailAddress;
        Password = password;
    }

    [CommandOption("--selenium-timeout-ms")]
    public int SeleniumTimeoutMs { set; get; } = 20;

    [CommandOption("--download-interval-ms")]
    public int IntervalMs { set; get; } = 3000;
    
    [CommandOption("--mail-address")]
    public string MailAddress { set; get; }
    
    [CommandOption("--password")]
    public string Password { set; get; }
    
    [CommandOption("--start")]
    public DateOnly Start { set; get; }

    [CommandOption("--month-count")] 
    public int MonthCount { set; get; } = 1;

    public override ValidationResult Validate()
    {
        if (Start == DateOnly.MinValue)
        {
            return ValidationResult.Error("日付が正しくありません");
        }
        if (string.IsNullOrWhiteSpace(MailAddress) || string.IsNullOrWhiteSpace(Password))
        {
            return ValidationResult.Error("メールアドレスとパスワードは省略することはできません");
        }
        return base.Validate();
    }
}