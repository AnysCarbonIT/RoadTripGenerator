using RoadTripGenerator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface IDeleteCountryUseCase
    {
        Task<RoadTrip> ExecuteAsync(string userId, string countryCca3, CancellationToken cancellationToken);
    }
}
