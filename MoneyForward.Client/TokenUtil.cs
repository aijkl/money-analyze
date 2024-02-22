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
        options.AddArgument("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.1 Safari/605.1.15");
        options.PageLoadStrategy = PageLoadStrategy.Normal;
        
        if (headless)
        {
            options.AddArgument("--mute-audio");
            options.AddArgument("--headless");
        }

        _chromeDriver = new ChromeDriver(_service, options);
        _webDriverWait = new WebDriverWait(_chromeDriver, timeoutSpan);
        _chromeDriver.Manage().Window.Maximize();
    }
    public string Login(string email, string password)
    {
        try
        {
            _chromeDriver.Navigate().GoToUrl("https://id.moneyforward.com/sign_in");

            var mailElement = _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@name='mfid_user[email]']")));
            mailElement.SendKeys(email);
            _chromeDriver.FindElement(By.XPath(@"//*[@id=""submitto""]")).Click();

            var passwordElement = _webDriverWait.Until(driver => driver.FindElement(By.XPath("//input[@name='mfid_user[password]']")));
            passwordElement.SendKeys(password);

            _chromeDriver.FindElement(By.XPath(@"//*[@id=""submitto""]")).Click();

            _chromeDriver.Navigate().GoToUrl("https://moneyforward.com/sign_in/");
            _webDriverWait.Until(driver => driver.FindElement(By.XPath("/html/body/main/div/div/div[2]/div/section/div/div/form/button"))).Click();
            try
            {
                _webDriverWait.Until(driver => driver.FindElement(By.XPath("/html/body/main/div/div/div[2]/div/section/div/a"))).Click();
            }
            catch { /* ignored */ }

            _webDriverWait.Until(driver => driver.FindElement(By.Id("page-home")));
            return _chromeDriver.Manage().Cookies.AllCookies.FirstOrDefault(x => x.Name == "_moneybook_session")?.Value ?? throw new ApplicationException();
        }
        catch (Exception e)
        {
            File.WriteAllBytes("dump.png", _chromeDriver.GetScreenshot().AsByteArray);
            File.WriteAllText("error", e.ToString());
            throw;
        }
    }

    public void Dispose()
    {
        _chromeDriver.Dispose();
        _service.Dispose();
    }
}