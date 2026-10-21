using Microsoft.EntityFrameworkCore;
using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;

namespace RoadTripGenerator.Infrastructure.Persistence.DataAccess
{
    public class RoadTripRepository : IRoadTripRepository
    {
        private readonly RoadTripGeneratorDbContext _dbContext;

        public RoadTripRepository(RoadTripGeneratorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<RoadTrip?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _dbContext.Roadtrips
                .AsNoTracking()
                .Include(roadTrip => roadTrip.Steps)
                .FirstOrDefaultAsync(roadTrip => roadTrip.UserId == userId, cancellationToken);
        }
    }
}
