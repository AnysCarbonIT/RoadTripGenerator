using System.ComponentModel.DataAnnotations;

namespace RoadTripGenerator.Api.DTO
{
    public record ReplaceRoadTripRequestDto
    {
        [Required]
        public required List<string> Countries { get; init; }
    }
}
