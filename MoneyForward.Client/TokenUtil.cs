using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MoneyForward.Client;

public class TokenUtil : IDisposable
{
    private readonly ChromeDriver _chromeDriver;
    private readonly ChromeDriverService _service;
    private readonly WebDriverWait _webDriverWait;
    public TokenUtil(bool headless, TimeSpan timeoutSpan)
    {
        _service = ChromeDriverService.CreateDefaultService();
        _service.EnableVerboseLogging = false;
        _service.EnableAppendLog = false;
        _service.HideCommandPromptWindow = true;

        var options = new ChromeOptions();
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-crash-reporter");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-in-process-stack-traces");
        options.AddArgument("--disable-logging");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--output=/dev/null");
        options.AddArgument("--log-level=3l");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.PageLoadStrategy = PageLoadStrategy.Normal;
        
        if (headless)
        {
            options.AddArgument("--mute-audio");
            options.AddArgument("--headless");
        }

        _chromeDriver = new ChromeDriver(_service, options);
        _webDriverWait = new WebDriverWait(_chromeDriver, timeoutSpan);
        
        if (!headless)
        {
            _chromeDriver.Manage().Window.Maximize();
        }
    }
    public string Login(string email, string password)
    {
        _chromeDriver.Navigate().GoToUrl("https://id.moneyforward.com/sign_in/email");

        var mailElement = _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@name='mfid_user[email]']")));
        mailElement.SendKeys(email);
        _chromeDriver.FindElement(By.XPath("//input[@value='同意してログインする']")).Click();
        
        var passwordElement = _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@name='mfid_user[password]']")));
        passwordElement.SendKeys(password);
        
        _chromeDriver.FindElement(By.XPath("//input[@value='ログインする']")).Click();
        
        _chromeDriver.Navigate().GoToUrl("https://moneyforward.com/sign_in/");
        _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@value='このアカウントを使用する']"))).Click();
        _webDriverWait.Until(driver => driver.FindElement(By.Id("page-home")));
        return _chromeDriver.Manage().Cookies.AllCookies.FirstOrDefault(x => x.Name == "_moneybook_session")?.Value ?? throw new ApplicationException();
    }

    public void Dispose()
    {
        _chromeDriver.Dispose();
        _service.Dispose();
    }
}