using RoadTripGenerator.Api.DTO;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Api.Mappers
{
    public  class CountryResponseMapper
    {
        public  CountryResponseDto MapToResponse(Country country)
        {
            ArgumentNullException.ThrowIfNull(country);

            return new CountryResponseDto
            {
                Name = new CountryNameResponseDto
                {
                    Common = country.Name
                },
                Cca3 = country.Cca3,
                Capital = country.CapitalCities.ToList(),
                Region = country.Region,
                Population = country.Population,
                Flag = country.Flag
            };
        }
    }
}
