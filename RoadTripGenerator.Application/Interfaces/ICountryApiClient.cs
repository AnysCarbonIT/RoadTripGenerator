using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface ICountryApiClient
    {
        Task<IReadOnlyList<Country>> GetAllCountriesAsync(CancellationToken cancellationToken); 
        // CountryApiClient expose la methde GetAllCountriesAsync
    }
}
