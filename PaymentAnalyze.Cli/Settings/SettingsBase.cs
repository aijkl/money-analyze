using Spectre.Console;
using Spectre.Console.Cli;
// ReSharper disable RedundantIfElseBlock

namespace PaymentAnalyze.Cli.Settings;

public class SettingsBase : CommandSettings
{
    protected SettingsBase(string mailAddress, string password, string cacheFilePath)
    {
        MailAddress = mailAddress;
        Password = password;
        CacheFilePath = cacheFilePath;
    }

    [CommandOption("--mail-address")]
    public string MailAddress { set; get; }
    
    [CommandOption("--password")]
    public string Password { set; get; }
    
    [CommandOption("--cache-file-path")] 
    public string CacheFilePath { set; get; }
    
    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(MailAddress) || string.IsNullOrEmpty(Password))
        {
            return ValidationResult.Error("メールアドレスまたはパスワードが正しくありません");
        } 
        else if(string.IsNullOrEmpty(CacheFilePath))
        {
            return ValidationResult.Error("キャッシュファイルパスが正しくありません");
        }
        return ValidationResult.Success();
    }
}