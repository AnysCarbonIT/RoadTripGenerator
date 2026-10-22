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

        public async Task<RoadTrip> ReplaceAsync(string userId, IReadOnlyList<Step> steps, CancellationToken cancellationToken)
        {
            var roadTrip = await _dbContext.Roadtrips
                .Include(roadTrip => roadTrip.Steps)
                .FirstOrDefaultAsync(roadTrip => roadTrip.UserId == userId, cancellationToken);
            // Transaction pour que chaque etape soit reussi ( suppression des ancien steps et ajout des noiveau)
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (roadTrip == null)
                {
                    roadTrip = new RoadTrip { UserId = userId };
                    _dbContext.Roadtrips.Add(roadTrip);
                }
                else
                {
                    _dbContext.Steps.RemoveRange(roadTrip.Steps);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    roadTrip.Steps.Clear();
                }

                foreach (Step step in steps)
                {
                    roadTrip.Steps.Add(step);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return roadTrip;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
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
