using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface IReplaceRoadTripUseCase
    {
        Task<RoadTrip> ExecuteAsync(string userId,IReadOnlyList<string> countries,CancellationToken cancellationToken);
    }
}
