using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Application.UseCases
{
    public class DeleteCountryUseCase : IDeleteCountryUseCase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IRoadTripRepository _roadTripRepository;
        public DeleteCountryUseCase(ICountryRepository countryRepository, IRoadTripRepository roadTripRepository)
        {
            _countryRepository = countryRepository;
            _roadTripRepository = roadTripRepository;
        }
        public async Task<RoadTrip> ExecuteAsync(string userId, string countryCca3, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(countryCca3))
            {
                throw new ArgumentException("Le code pays est obligatoire.");
            }
            if (!await _countryRepository.AllCountriesExistAsync(new List<string> { countryCca3 }, cancellationToken))
            {
                throw new ArgumentException("Le code pays est inconnu.");
            }
            return await _roadTripRepository.DeleteCountryAsync(userId, countryCca3, cancellationToken);
        }

    }
}
