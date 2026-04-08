using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace BddDotNetLab.Steps
{
    [Binding]
    public class SearchSteps
    {
        private IWebDriver _driver = null!;

        [BeforeScenario]
        public void SetUp()
        {
            var options = new ChromeOptions();
            // Ejecutar Chrome/Chromium en modo silencioso (headless) esencial para Codespaces
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox"); // Necesario para correr dentro de Docker/Codespaces
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--remote-allow-origins=*");

            // Indicamos la ruta del ChromeDriver instalado por la feature "chrometesting" del DevContainer
            _driver = new ChromeDriver("/usr/local/bin", options);

            // Espera implícita para que la página renderice los elementos
            _driver.Manage().Timeouts().ImplicitWait = System.TimeSpan.FromSeconds(10);
        }

        [Given(@"I am on the Google search page")]
        public void GivenIAmOnTheGoogleSearchPage()
        {
            _driver.Navigate().GoToUrl("https://www.google.com");
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string term)
        {
            var searchBox = _driver.FindElement(By.Name("q"));
            searchBox.SendKeys(term);
            searchBox.Submit();
        }

        [Then(@"I should see ""(.*)"" in the results")]
        public void ThenIShouldSeeInTheResults(string term)
        {
            // Esperar a que Google termine de cargar los resultados dinámicamente
            System.Threading.Thread.Sleep(2000);

            string pageSource = _driver.PageSource;
            Assert.IsTrue(pageSource.Contains(term), $"No se encontró el término '{term}' en la página.");
        }

        [AfterScenario]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}