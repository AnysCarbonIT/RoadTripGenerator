using RoadTripGenerator.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Infrastructure.Persistence.DataAccess
{
    public class CountryInitializationService
    {
        private readonly ICountryApiClient _countryApiClient;
        private readonly ICountryRepository _countryRepository;
        public CountryInitializationService(ICountryApiClient countryApiClient, ICountryRepository countryRepository)
        {
                    _countryApiClient = countryApiClient;
                    _countryRepository = countryRepository;       
        }
        public async Task InitializeCountriesAsync(CancellationToken cancellationToken)
        {
            if (!await _countryRepository.HasCountriesAsync(cancellationToken))
            {
                var countries = await _countryApiClient.GetAllCountriesAsync(cancellationToken);
                await _countryRepository.AddCountryAsync(countries, cancellationToken);
            }
        }
    }
}
