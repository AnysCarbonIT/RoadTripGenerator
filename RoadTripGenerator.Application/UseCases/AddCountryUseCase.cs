using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace RoadTripGenerator.Application.UseCases
{
    public class AddCountryUseCase : IAddCountryUseCase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IRoadTripRepository _roadTripRepository;
        public AddCountryUseCase(ICountryRepository countryRepository, IRoadTripRepository roadTripRepository)
        {
            _countryRepository = countryRepository;
            _roadTripRepository = roadTripRepository;
        }
        public async Task <RoadTrip> ExecuteAsync(string userId, string countryCca3, CancellationToken cancellationToken)
        {
            Country.ValidateCountryCode(countryCca3);

            if (!await _countryRepository.AllCountriesExistAsync(new List<string> { countryCca3 },cancellationToken))
            {
                throw new DomainValidationException("Le code pays est inconnu.");
            }
            var roadTrip = await _roadTripRepository.GetByUserIdAsync( userId, cancellationToken);

            bool isNew = roadTrip == null;

            roadTrip ??= new RoadTrip(userId);

            roadTrip.AddCountry(countryCca3);

            if (isNew)
            {
                _roadTripRepository.Add(roadTrip);
            } // car non suivi par EF Core, donc on doit l'ajouter explicitement (nouvelle entité) 

            await _roadTripRepository.SaveChangesAsync(cancellationToken);

            return roadTrip;

        }
    }
}
