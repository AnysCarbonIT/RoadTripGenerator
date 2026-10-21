namespace RoadTripGenerator.Api.DTO
{
    public record CountryResponseDto
    {
        public required CountryNameResponseDto Name { get; init; }
        public required string Cca3 { get; init; }
        public required List<string> Capital { get; init; }
        public required string Region { get; init; }
        public required long Population { get; init; }
        public required string Flag { get; init; }
    }

    public record CountryNameResponseDto
    {
        public required string Common { get; init; }
    }
}
