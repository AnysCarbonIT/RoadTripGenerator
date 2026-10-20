using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface ICountryRepository
    {
        //Asynchrone car interoge la db
        Task<bool> HasCountriesAsync(CancellationToken cancellationToken);
        Task AddCountryAsync(IReadOnlyList<Country> countries, CancellationToken cancellationToken);
    }
}
