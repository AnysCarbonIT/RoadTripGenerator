using RoadTripGenerator.Domain.Exceptions;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.IntegrationTests
{
    public class CountryTests
    {
        [Test]
        public void CreateCountry_EmptyName_IsRejected()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Country("FRA", "", "Europe", 68_000_000,
                    new List<string> { "Paris" }, "🇫🇷"));
        }

        [Test]
        public void CreateCountry_NegativePopulation_IsRejected()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Country("FRA", "France", "Europe", -1,
                    new List<string> { "Paris" }, "🇫🇷"));
        }

        [Test]
        public void CapitalCities_ChangingOriginalList_DoesNotChangeCountry()
        {
            var capitals = new List<string> { "Paris" };
            var country = new Country("FRA", "France", "Europe", 68_000_000, capitals, "🇫🇷");

            capitals.Add("Lyon");

            Assert.That(country.CapitalCities.Count, Is.EqualTo(1));
            Assert.That(country.CapitalCities[0], Is.EqualTo("Paris"));
        }
    }
}
