
namespace RoadTripGenerator.Domain.Models
{
    public class Country
    {
        public required string Cca3 { get; set; }
        public required string Name { get; set; }
        public required string Region { get; set; }
        public required long Population { get; set; }
        public  List<string> CapitalCities { get; set; } = new List<string>();
        public required string Flag { get; set; }

    }
}
