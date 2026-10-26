using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RoadTripGenerator.IntegrationTests
{
    public class CountriesTests
    {
        private RoadTripApiFactory _factory = null!;
        private HttpClient _client = null!;

        [OneTimeSetUp] //methode qui se lance avant les tests
        public async Task SetupAsync()
        {
            _factory = new RoadTripApiFactory();
            await _factory.InitializeDatabaseAsync(); // initialise la base
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions{BaseAddress = new Uri("https://localhost")});
        }

        [Test]
        public async Task GetCountries_PageTwoWithOneCountry_ReturnsFrance()
        {
            using var response = await _client.GetAsync("/api/countries?page=2&pageSize=1");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.That(json.RootElement.GetArrayLength(), Is.EqualTo(1));

            var country = json.RootElement[0];
            using (Assert.EnterMultipleScope())
            {
                Assert.That(country.GetProperty("cca3").GetString(), Is.EqualTo("FRA"));
                Assert.That(country.GetProperty("name").GetProperty("common").GetString(), Is.EqualTo("France"));
                Assert.That(country.GetProperty("capital")[0].GetString(), Is.EqualTo("Paris"));
            }
        }

        [OneTimeTearDown]
        public async Task CleanupAsync()
        {
            _client?.Dispose();
            if (_factory != null)
            {
                await _factory.DisposeAsync();
            }
        }
    }
}
