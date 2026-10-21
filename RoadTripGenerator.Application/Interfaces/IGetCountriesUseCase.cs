using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface IGetCountriesUseCase
    {
        Task<IReadOnlyList<Country>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken);
    }
}
