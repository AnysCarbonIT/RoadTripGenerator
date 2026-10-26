using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Infrastructure.ExternalUpload
{
    public class CountryMapper
    {
        public Country MapToCountry(RestCountryDto restCountryDto)
        {
            if (restCountryDto == null)
            {
                throw new ArgumentNullException(nameof(restCountryDto));
            }
            if (string.IsNullOrWhiteSpace(restCountryDto.Codes?.Alpha3))
            {
                throw new ArgumentException("Le code CCA3 du pays est manquant.");
            }

            if (string.IsNullOrWhiteSpace(restCountryDto.Names?.Common))
            {
                throw new ArgumentException("Le nom du pays est manquant.");
            }

            if (string.IsNullOrWhiteSpace(restCountryDto.Region))
            {
                throw new ArgumentException("La région du pays est manquante.");
            }

            if (string.IsNullOrWhiteSpace(restCountryDto.Flag?.Emoji))
            {
                throw new ArgumentException("Le drapeau du pays est manquant.");
            }
            return new Country(
                cca3: restCountryDto.Codes.Alpha3,
                name: restCountryDto.Names.Common,
                region: restCountryDto.Region,
                flag: restCountryDto.Flag.Emoji,
                population: restCountryDto.Population,
                capitalCities: restCountryDto.Capitals?
                    .Where(capital => !string.IsNullOrWhiteSpace(capital.Name))
                    .Select(capital => capital.Name!)
                    .ToList() ?? new List<string>());
        }
    }
}
