using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Domain.Exceptions;

namespace RoadTripGenerator.Application.UseCases
{
    public class ReplaceRoadTripUseCase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IRoadTripRepository _roadTripRepository;

        public ReplaceRoadTripUseCase(ICountryRepository countryRepository, IRoadTripRepository roadTripRepository)
        {
            _countryRepository = countryRepository;
            _roadTripRepository = roadTripRepository;
        }

        public async Task<RoadTrip> ExecuteAsync(string userId, IReadOnlyList<string> countries, CancellationToken cancellationToken)
        {
            var roadTrip = await _roadTripRepository.GetByUserIdAsync(userId, cancellationToken);
            bool isNew = roadTrip == null;
            roadTrip ??= new RoadTrip(userId);

            // Le Domain valide toute la liste avant de modifier les étapes.
            roadTrip.ReplaceCountries(countries);

            if (countries.Count > 0 && !await _countryRepository.AllCountriesExistAsync(countries, cancellationToken))
            {
                throw new DomainValidationException("Un ou plusieurs codes pays sont inconnus.");
            }

            if (isNew)
            {
                _roadTripRepository.Add(roadTrip);
            } // car non suivi par EF Core, donc on doit l'ajouter explicitement (nouvelle entité) 

            await _roadTripRepository.SaveChangesAsync(cancellationToken);
            return roadTrip;
        }
    }
}
