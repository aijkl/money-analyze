using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MoneyForward.Client;

public class TokenUtil : IDisposable
{
    private readonly ChromeDriverService _service;
    private readonly WebDriverWait _webDriverWait;
    public readonly ChromeDriver ChromeDriver;
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
        options.AddArgument("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Safari/605.1.15");
        options.PageLoadStrategy = PageLoadStrategy.Normal;
        
        if (headless)
        {
            options.AddArgument("--mute-audio");
            options.AddArgument("--headless");
        }

        ChromeDriver = new ChromeDriver(_service, options);
        _webDriverWait = new WebDriverWait(ChromeDriver, timeoutSpan);
        ChromeDriver.Manage().Window.Maximize();
    }
    public string Login(string email, string password)
    {
        ChromeDriver.Navigate().GoToUrl("https://id.moneyforward.com/sign_in");

        var mailElement = _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@name='mfid_user[email]']")));
        mailElement.SendKeys(email);
        ChromeDriver.FindElement(By.XPath(@"//*[@id=""submitto""]")).Click();

        var passwordElement = _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@name='mfid_user[password]']")));
        passwordElement.SendKeys(password);

        ChromeDriver.FindElement(By.XPath(@"//*[@id=""submitto""]")).Click();

        ChromeDriver.Navigate().GoToUrl("https://moneyforward.com/sign_in/");
        _webDriverWait.Until(driver => driver.FindElement(By.XPath("/html/body/main/div/div/div[2]/div/section/div/div/form/button"))).Click();
        try
        {
            _webDriverWait.Until(driver => driver.FindElement(By.XPath("/html/body/main/div/div/div[2]/div/section/div/a"))).Click();
        }
        catch { /* ignored */ }

        _webDriverWait.Until(driver => driver.FindElement(By.Id("page-home")));
        return ChromeDriver.Manage().Cookies.AllCookies.FirstOrDefault(x => x.Name == "_moneybook_session")?.Value ?? throw new ApplicationException();
    }

    public void Dispose()
    {
        ChromeDriver.Dispose();
        _service.Dispose();
    }
}