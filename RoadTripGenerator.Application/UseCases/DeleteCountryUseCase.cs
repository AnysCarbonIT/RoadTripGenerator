using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Application.Exceptions;

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
            Country.ValidateCountryCode(countryCca3);
            var roadTrip = await _roadTripRepository.GetByUserIdAsync(userId, cancellationToken) ?? throw new RoadTripNotFoundException();

            roadTrip.RemoveCountry(countryCca3);
            await _roadTripRepository.SaveChangesAsync(cancellationToken);
            return roadTrip;
        }
    }
}
