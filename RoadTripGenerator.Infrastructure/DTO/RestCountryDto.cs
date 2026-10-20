using System.Text.Json.Serialization;

namespace RoadTripGenerator.Infrastructure.DTO
{
    public record RestCountryDto
    {
        [JsonPropertyName("names")]
        public RestCountryNameDto? Names { get; set; }

        [JsonPropertyName("codes")]
        public RestCountryCodesDto? Codes { get; set; }

        [JsonPropertyName("capitals")]
        public List<RestCountryCapitalDto> Capitals { get; set; }
            = new List<RestCountryCapitalDto>();

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("population")]
        public long Population { get; set; }

        [JsonPropertyName("flag")]
        public RestCountryFlagDto? Flag { get; set; }
    }

    public record RestCountryNameDto
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }
    }

    public record RestCountryCodesDto
    {
        [JsonPropertyName("alpha_3")]
        public string? Alpha3 { get; set; }
    }

    public record RestCountryCapitalDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public record RestCountryFlagDto
    {
        [JsonPropertyName("emoji")]
        public string? Emoji { get; set; }
    }
}