using MoneyForward.Client;
using PaymentAnalyze.Cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PaymentAnalyze.Cli.Commands;

public abstract class CommandBase<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings
{
    private readonly TimeSpan _loginExpiredTime = TimeSpan.FromDays(20);
    protected async void LoginIfNeed(CacheData cacheData, SettingsBase settingsBase)
    {
        if (DateTime.Now - cacheData.LastLogin <= _loginExpiredTime) return;
        using var tokenUtil = new TokenUtil(true, TimeSpan.FromSeconds(30));
        try
        {
            var timestamp = DateTime.Now;
            cacheData.Token = tokenUtil.Login(settingsBase.MailAddress, settingsBase.Password);
            cacheData.LastLogin = timestamp;
            cacheData.SaveToFile();
        }
        catch (Exception e)
        {
            var fullPath = Path.GetFullPath("./error.png");
            await File.WriteAllBytesAsync(fullPath, tokenUtil.ChromeDriver.GetScreenshot().AsByteArray);
            AnsiConsoleHelper.MarkupLine($"ログインに失敗しましたため、スクリーンショットを保存しました Path: ${fullPath}", AnsiConsoleHelper.State.Failure);
            AnsiConsole.WriteException(e);
            throw;
        }
    }
}