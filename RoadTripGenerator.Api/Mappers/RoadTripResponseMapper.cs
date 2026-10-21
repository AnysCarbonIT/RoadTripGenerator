using RoadTripGenerator.Api.DTO;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Api.Mappers
{
    public class RoadTripResponseMapper
    {
        public RoadTripResponseDto MapToResponse(RoadTrip? roadTrip)
        {
            if (roadTrip == null)
            {
                return new RoadTripResponseDto();
            }

            return new RoadTripResponseDto
            {
                Countries = roadTrip.Steps
                    .OrderBy(step => step.Order)
                    .Select(step => new RoadTripCountryResponseDto
                    {
                        Cca3 = step.CountryCca3,
                        Order = step.Order
                    })
                    .ToList()
            };
        }
    }
}
