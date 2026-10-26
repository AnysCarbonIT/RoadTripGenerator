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
            // Le suivi EF permet de sauvegarder les modifications faites par RoadTrip.
            return _dbContext.Roadtrips
                .Include(roadTrip => roadTrip.Steps)
                .FirstOrDefaultAsync(roadTrip => roadTrip.UserId == userId, cancellationToken);
        }

        public void Add(RoadTrip roadTrip)
        {
            _dbContext.Roadtrips.Add(roadTrip);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            // Une sauvegarde EF valide les modifications dans une même transaction.
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
