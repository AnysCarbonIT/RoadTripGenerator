using System.Text.Json.Serialization;

namespace RoadTripGenerator.Infrastructure.DTO
{
    public class RestCountryDto
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

    public class RestCountryNameDto
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }
    }

    public class RestCountryCodesDto
    {
        [JsonPropertyName("alpha_3")]
        public string? Alpha3 { get; set; }
    }

    public class RestCountryCapitalDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class RestCountryFlagDto
    {
        [JsonPropertyName("emoji")]
        public string? Emoji { get; set; }
    }
}