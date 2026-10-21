using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface IGetRoadTripUseCase
    {
        Task<RoadTrip?> ExecuteAsync(string userId, CancellationToken cancellationToken);
    }
}
