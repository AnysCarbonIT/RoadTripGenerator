using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RoadTripGenerator.IntegrationTests
{
    public class RoadTripsTests
    {
        private RoadTripApiFactory _factory = null!;
        private HttpClient _client = null!;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            // Prépare PostgreSQL et démarre l'API pour les tests.
            _factory = new RoadTripApiFactory();
            await _factory.InitializeDatabaseAsync();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }

        [SetUp]
        public void UseNewUser()
        {
            // Un utilisateur différent à chaque test évite de partager un itinéraire
            _client.DefaultRequestHeaders.Remove("X-User-ID");
            _client.DefaultRequestHeaders.Add("X-User-ID", $"test-{Guid.NewGuid():N}");
        }

        [Test]
        public async Task PutRoadTrip_ReplacesCountries()
        {
            //  prépare un itinéraire existant
            using var initialResponse = await _client.PutAsJsonAsync("/api/roadtrip",new { countries = new[] { "DEU", "FRA" } });
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // remplacer cet itinéraire par la Croatie et l'Allemagne
            using var response = await _client.PutAsJsonAsync("/api/roadtrip",new { countries = new[] { "HRV", "DEU" } });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // relire l'itinéraire pour vérifier ce qui a été enregistré
            using var savedResponse = await _client.GetAsync("/api/roadtrip");
            Assert.That(savedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var json = JsonDocument.Parse(await savedResponse.Content.ReadAsStringAsync());
            var countries = json.RootElement.GetProperty("countries");

            Assert.That(countries.GetArrayLength(), Is.EqualTo(2));
            Assert.That(countries[0].GetProperty("cca3").GetString(), Is.EqualTo("HRV"));
            Assert.That(countries[1].GetProperty("cca3").GetString(), Is.EqualTo("DEU"));
        }

        [Test]
        public async Task DeleteCountry_RemovesCountryFromRoadTrip()
        {
            // prépare un itinéraire contenant deux pays.
            using var initialResponse = await _client.PutAsJsonAsync("/api/roadtrip",new { countries = new[] { "FRA", "DEU" } });
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // supprimer la France de cet itinéraire.
            using var response = await _client.DeleteAsync("/api/roadtrip/country/FRA");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // relire l'itinéraire avec Allemagne seul en order 1
            using var savedResponse = await _client.GetAsync("/api/roadtrip");
            Assert.That(savedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var json = JsonDocument.Parse(await savedResponse.Content.ReadAsStringAsync());
            var countries = json.RootElement.GetProperty("countries");

            Assert.That(countries.GetArrayLength(), Is.EqualTo(1));
            Assert.That(countries[0].GetProperty("cca3").GetString(), Is.EqualTo("DEU"));
            Assert.That(countries[0].GetProperty("order").GetInt32(), Is.EqualTo(1));
        }

        [TestCase("HRV", "FRA", "DEU")]
        [TestCase("FRA", "HRV", "DEU")]
        [TestCase("DEU", "HRV", "FRA")]
        public async Task DeleteCountry_RenumbersRemainingSteps(string removed, string first, string second)
        {
            using var initialResponse = await _client.PutAsJsonAsync("/api/roadtrip",
                new { countries = new[] { "HRV", "FRA", "DEU" } });
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var response = await _client.DeleteAsync($"/api/roadtrip/country/{removed}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var savedResponse = await _client.GetAsync("/api/roadtrip");
            Assert.That(savedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var json = JsonDocument.Parse(await savedResponse.Content.ReadAsStringAsync());
            var countries = json.RootElement.GetProperty("countries");
            Assert.That(countries.GetArrayLength(), Is.EqualTo(2));
            Assert.That(countries[0].GetProperty("cca3").GetString(), Is.EqualTo(first));
            Assert.That(countries[0].GetProperty("order").GetInt32(), Is.EqualTo(1));
            Assert.That(countries[1].GetProperty("cca3").GetString(), Is.EqualTo(second));
            Assert.That(countries[1].GetProperty("order").GetInt32(), Is.EqualTo(2));
        }

        [Test]
        public async Task PostCountry_AddsCountryAtEnd()
        {
            using var initialResponse = await _client.PutAsJsonAsync("/api/roadtrip",
                new { countries = new[] { "FRA" } });
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var response = await _client.PostAsJsonAsync("/api/roadtrip/country", new { cca3 = "HRV" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var savedResponse = await _client.GetAsync("/api/roadtrip");
            Assert.That(savedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var json = JsonDocument.Parse(await savedResponse.Content.ReadAsStringAsync());
            var countries = json.RootElement.GetProperty("countries");
            Assert.That(countries.GetArrayLength(), Is.EqualTo(2));
            Assert.That(countries[0].GetProperty("cca3").GetString(), Is.EqualTo("FRA"));
            Assert.That(countries[1].GetProperty("cca3").GetString(), Is.EqualTo("HRV"));
            Assert.That(countries[1].GetProperty("order").GetInt32(), Is.EqualTo(2));
        }

        [TestCase("FRA", "FRA")]
        [TestCase("FRA", "ZZZ")]
        [TestCase("FRA", "invalid")]
        public async Task PutRoadTrip_InvalidCountries_DoesNotChangeSavedRoadTrip(string first, string second)
        {
            using var initialResponse = await _client.PutAsJsonAsync("/api/roadtrip",
                new { countries = new[] { "DEU" } });
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var response = await _client.PutAsJsonAsync("/api/roadtrip",
                new { countries = new[] { first, second } });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            using var savedResponse = await _client.GetAsync("/api/roadtrip");
            Assert.That(savedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var json = JsonDocument.Parse(await savedResponse.Content.ReadAsStringAsync());
            var countries = json.RootElement.GetProperty("countries");
            Assert.That(countries.GetArrayLength(), Is.EqualTo(1));
            Assert.That(countries[0].GetProperty("cca3").GetString(), Is.EqualTo("DEU"));
        }

        [Test]
        public async Task PutRoadTrip_EmptyCountries_ClearsRoadTrip()
        {
            using var initialResponse = await _client.PutAsJsonAsync("/api/roadtrip",
                new { countries = new[] { "FRA", "DEU" } });
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var response = await _client.PutAsJsonAsync("/api/roadtrip",
                new { countries = Array.Empty<string>() });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            using var savedResponse = await _client.GetAsync("/api/roadtrip");
            Assert.That(savedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var json = JsonDocument.Parse(await savedResponse.Content.ReadAsStringAsync());
            Assert.That(json.RootElement.GetProperty("countries").GetArrayLength(), Is.EqualTo(0));
        }

        [TestCase("FRA", HttpStatusCode.NotFound)]
        [TestCase("invalid", HttpStatusCode.BadRequest)]
        public async Task DeleteCountry_WithoutRoadTrip_ReturnsProblemDetails(string code, HttpStatusCode expectedStatus)
        {
            using var response = await _client.DeleteAsync($"/api/roadtrip/country/{code}");
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatus));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo((int)expectedStatus));
            Assert.That(json.RootElement.GetProperty("traceId").GetString(), Is.Not.Empty);
        }

        [Test]
        public async Task DeleteCountry_NotInRoadTrip_ReturnsNotFound()
        {
            using var initial = await _client.PutAsJsonAsync("/api/roadtrip", new { countries = new[] { "DEU" } });
            Assert.That(initial.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var response = await _client.DeleteAsync("/api/roadtrip/country/FRA");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
        }

        [TestCase("FRA")]
        [TestCase("ZZZ")]
        [TestCase("fra")]
        public async Task PostCountry_InvalidAddition_ReturnsBadRequest(string code)
        {
            using var initial = await _client.PutAsJsonAsync("/api/roadtrip", new { countries = new[] { "FRA" } });
            Assert.That(initial.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            using var response = await _client.PostAsJsonAsync("/api/roadtrip/country", new { cca3 = code });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.That(json.RootElement.GetProperty("traceId").GetString(), Is.Not.Empty);
        }

        [Test]
        public async Task GetRoadTrip_WithoutUserHeader_ReturnsUnauthorizedProblem()
        {
            _client.DefaultRequestHeaders.Remove("X-User-ID");
            using var response = await _client.GetAsync("/api/roadtrip");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
        }

        [OneTimeTearDown]
        public async Task CleanupAsync()
        {
            // Libère le client HTTP et arrête l'API de test.
            _client?.Dispose();
            if (_factory != null)
            {
                await _factory.DisposeAsync();
            }
        }
    }
}
