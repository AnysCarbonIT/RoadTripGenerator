using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using System.Net.Http.Json;
using RoadTripGenerator.Infrastructure.DTO;

namespace RoadTripGenerator.Infrastructure.ExternalUpload
{
    public class RestCountryClient : ICountryApiClient
    {
        private readonly HttpClient _httpClient;

        public RestCountryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<Country>> GetAllCountriesAsync(
            CancellationToken cancellationToken)
        {
            List<Country> countries = new List<Country>();

            var mapper = new CountryMapper();

            const int limit = 100;
            int offset = 0;
            bool hasMore = true;

            while (hasMore)
            {
                string url =
                    $"countries/v5?limit={limit}&offset={offset}" +
                    "&response_fields=names.common,codes.alpha_3," +
                    "capitals,region,population,flag.emoji";

                RestCountriesResponseDto? result =
                    await _httpClient.GetFromJsonAsync<RestCountriesResponseDto>(
                        url, cancellationToken);

                if (result?.Data == null)
                {
                    throw new InvalidDataException("La réponse REST Countries ne contient pas de données.");
                }
                if (result.Data.Meta?.More == null)
                {
                    throw new InvalidDataException("Métadonnées de pagination manquantes.");
                }

                if (result.Data.Objects == null || result.Data.Objects.Count == 0)
                {
                    throw new InvalidDataException("Une page REST Countries est vide.");
                }

                foreach (RestCountryDto countryDto in result.Data.Objects)
                {
                    // Exlue les Pays sans code CCA3
                    if (string.IsNullOrWhiteSpace(countryDto.Codes?.Alpha3))
                    {
                        Console.WriteLine(
                            $"Entrée ignorée : {countryDto.Names?.Common}, code CCA3 absent.");
                        continue;
                    }

                    Country country = mapper.MapToCountry(countryDto);
                    countries.Add(country);
                }

                Console.WriteLine(
                    $"Page reçue : offset={offset}, " +
                    $"nombre de pays={result.Data.Objects.Count}, " +
                    $"reste des pages={result.Data.Meta.More}");

                offset += result.Data.Objects.Count;
                hasMore = result.Data.Meta.More.Value;
            }

            return countries;
        }
    }
}
