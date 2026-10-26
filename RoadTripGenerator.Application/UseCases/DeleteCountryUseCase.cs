using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.UseCases
{
    public class DeleteCountryUseCase : IDeleteCountryUseCase
    {
        private readonly IRoadTripRepository _roadTripRepository;

        public DeleteCountryUseCase(IRoadTripRepository roadTripRepository)
        {
            _roadTripRepository = roadTripRepository;
        }

        public async Task<RoadTrip> ExecuteAsync(string userId, string countryCca3, CancellationToken cancellationToken)
        {
            var roadTrip = await _roadTripRepository.GetByUserIdAsync(userId, cancellationToken) ?? throw new ArgumentException("Aucun itinéraire trouvé pour cet utilisateur.");

            roadTrip.RemoveCountry(countryCca3);
            await _roadTripRepository.SaveChangesAsync(cancellationToken);
            return roadTrip;
        }
    }
}
