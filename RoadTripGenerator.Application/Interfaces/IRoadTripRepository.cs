using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface IRoadTripRepository
    {
        Task<RoadTrip?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<RoadTrip> ReplaceAsync(string userId, IReadOnlyList<Step> steps, CancellationToken cancellationToken);
        Task<RoadTrip> AddCountryAsync(string userId, string coountryCca3, CancellationToken cancellationToken);
    }
}
