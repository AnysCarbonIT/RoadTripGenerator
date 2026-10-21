using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.UseCases
{
    public class GetCountriesUseCase : IGetCountriesUseCase
    {
        private readonly ICountryRepository _countryRepository;

        public GetCountriesUseCase(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public Task<IReadOnlyList<Country>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return _countryRepository.GetCountriesAsync(page, pageSize, cancellationToken);
        }
    }
}
