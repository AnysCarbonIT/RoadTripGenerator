using RoadTripGenerator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Application.Interfaces
{
    public interface IAddCountryUseCase
    {
        Task <RoadTrip>ExecuteAsync(string userId, string countryCca3, CancellationToken cancellationToken);
    }
}
