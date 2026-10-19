using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Infrastructure.DTO;

namespace RoadTripGenerator.Infrastructure.ExternalUpload
{

    public  class RestCountryClient : ICountryApiClient
    {
        private readonly HttpClient _httpClient;
        public RestCountryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IReadOnlyList<Country>> GetAllCountriesAsync(CancellationToken cancellationToken)
        {
            try {
                IReadOnlyList<Country> countries = new List<Country>();
                using var response = await _httpClient.GetAsync("countries/v5?limit=3&offset=0", cancellationToken);
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync(cancellationToken);
                var deserializer = new Deserialization();
                RestCountriesResponseDto result = deserializer.Deserialize(json);
                return countries;
            }
            catch(HttpRequestException ex)
            {
                throw new Exception("Erreur API", ex);
            }
            
        }
    }
}
