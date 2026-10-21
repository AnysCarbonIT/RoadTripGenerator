namespace RoadTripGenerator.Api.DTO
{
    public record RoadTripResponseDto
    {
        public List<RoadTripCountryResponseDto> Countries { get; init; } = new List<RoadTripCountryResponseDto>();
    }

    public record RoadTripCountryResponseDto
    {
        public required string Cca3 { get; init; }
        public required int Order { get; init; }
    }
}
