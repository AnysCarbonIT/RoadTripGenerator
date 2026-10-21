using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.UseCases
{
    public class GetRoadTripUseCase : IGetRoadTripUseCase
    {
        private readonly IRoadTripRepository _roadTripRepository;

        public GetRoadTripUseCase(IRoadTripRepository roadTripRepository)
        {
            _roadTripRepository = roadTripRepository;
        }

        public Task<RoadTrip?> ExecuteAsync(string userId, CancellationToken cancellationToken)
        {
            return _roadTripRepository.GetByUserIdAsync(userId, cancellationToken);
        }
    }
}
