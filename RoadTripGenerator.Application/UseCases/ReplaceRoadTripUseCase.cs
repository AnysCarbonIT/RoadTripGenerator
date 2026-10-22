using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.UseCases
{
    public class ReplaceRoadTripUseCase : IReplaceRoadTripUseCase
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
            if (countries == null)
            {
                throw new ArgumentException("La liste des pays est obligatoire.");
            }

            if (countries.Distinct().Count() != countries.Count)
            {
                throw new ArgumentException("Un pays ne peut pas apparaître plusieurs fois dans l'itinéraire.");
            }

            if (countries.Count > 0 && !await _countryRepository.AllCountriesExistAsync(countries, cancellationToken))
            {
                throw new ArgumentException("Un ou plusieurs codes pays sont inconnus.");
            }

            var steps = new List<Step>();
            for (int index = 0; index < countries.Count; index++)
            {
                var step = new Step { CountryCca3 = countries[index] };
                step.SetOrder(index + 1);
                steps.Add(step);
            }

            return await _roadTripRepository.ReplaceAsync(userId, steps, cancellationToken);
        }
    }
}
