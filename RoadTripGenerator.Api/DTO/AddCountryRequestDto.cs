namespace RoadTripGenerator.Api.DTO
{
    public record AddCountryRequestDto
    {
        public required string Cca3 { get; init; }
    }
}
