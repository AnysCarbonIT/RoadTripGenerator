using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Domain.Exceptions;

namespace RoadTripGenerator.IntegrationTests
{
    // Tests du Domain : aucune base de données nécessaire.
    public class CountryTests
    {
        [TestCase(null, "Europe", "🇫🇷")]
        [TestCase("   ", "Europe", "🇫🇷")]
        [TestCase("France", null, "🇫🇷")]
        [TestCase("France", "", "🇫🇷")]
        [TestCase("France", "Europe", null)]
        [TestCase("France", "Europe", " ")]
        public void Constructor_MissingRequiredText_RejectsCountry(string? name, string? region, string? flag)
        {
            Assert.Throws<DomainValidationException>(() =>
                new Country("FRA", name!, region!, 0, new List<string>(), flag!));
        }

        [Test]
        public void Constructor_NegativePopulation_RejectsCountry()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Country("FRA", "France", "Europe", -1, new List<string>(), "🇫🇷"));
        }

        [Test]
        public void Constructor_ZeroPopulationAndNoCapital_AcceptsCountry()
        {
            var country = new Country("FRA", "France", "Europe", 0, new List<string>(), "🇫🇷");
            Assert.That(country.Population, Is.Zero);
            Assert.That(country.CapitalCities, Is.Empty);
        }

        [Test]
        public void Constructor_NullCapitalList_RejectsCountry()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Country("FRA", "France", "Europe", 0, null!, "🇫🇷"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_EmptyCapital_RejectsCountry(string? capital)
        {
            Assert.Throws<DomainValidationException>(() =>
                new Country("FRA", "France", "Europe", 0, new List<string> { capital! }, "🇫🇷"));
        }

        [Test]
        public void CapitalCities_CannotBeModifiedThroughInputOrPublicCollection()
        {
            var capitals = new List<string> { "Paris" };
            var country = new Country("FRA", "France", "Europe", 0, capitals, "🇫🇷");

            capitals.Add("Lyon");
            Assert.That(country.CapitalCities, Is.EqualTo(new[] { "Paris" }));

            var exposedCollection = (ICollection<string>)country.CapitalCities;
            Assert.Throws<NotSupportedException>(() => exposedCollection.Add("Lyon"));
            Assert.That(country.CapitalCities, Is.EqualTo(new[] { "Paris" }));
        }
    }
}
