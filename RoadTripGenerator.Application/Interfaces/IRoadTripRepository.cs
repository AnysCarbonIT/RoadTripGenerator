using RoadTripGenerator.Domain.Models;
namespace RoadTripGenerator.Application.Interfaces
{
    public interface IRoadTripRepository
    {
        Task<RoadTrip?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        void Add(RoadTrip roadTrip);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
