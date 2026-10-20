using System.Text.Json.Serialization;

namespace RoadTripGenerator.Infrastructure.DTO
{
    public record RestCountriesResponseDto
    {
        [JsonPropertyName("data")]
        public RestCountriesDataDto? Data { get; set; }
    }

    public record RestCountriesDataDto
    {
        [JsonPropertyName("objects")]
        public List<RestCountryDto> Objects { get; set; } = new List<RestCountryDto>();

        [JsonPropertyName("meta")]
        public RestCountriesMetaDto? Meta { get; set; }
    }

    public record RestCountriesMetaDto
    {
        [JsonPropertyName("more")]
        public bool? More { get; set; }
    }
}