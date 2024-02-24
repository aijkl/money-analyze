using PaymentAnalyze.Cli.Commands;
using Spectre.Console.Cli;

public class Program
{
    public static int Main(string[] args)
    {
        var commandApp = new CommandApp();
        commandApp.Configure(x =>
        {
            x.AddCommand<UpdateRequestCommand>("update-request");
            x.AddCommand<DownloadCommand>("download");
        });
        return commandApp.Run(args);
    }
}