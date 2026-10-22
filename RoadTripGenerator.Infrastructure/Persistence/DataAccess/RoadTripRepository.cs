using Microsoft.EntityFrameworkCore;
using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using System.Reflection.Metadata.Ecma335;

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

        public async Task<RoadTrip?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
             return await _dbContext.Roadtrips
                .AsNoTracking()
                .Include(roadTrip => roadTrip.Steps)
                .FirstOrDefaultAsync(roadTrip => roadTrip.UserId == userId, cancellationToken);
        }

        public async Task<RoadTrip> AddCountryAsync(string userId, string coountryCca3, CancellationToken cancellationToken)
        {
            var roadTrip = await _dbContext.Roadtrips
                .Include(roadTrip => roadTrip.Steps)
                .FirstOrDefaultAsync(roadTrip => roadTrip.UserId == userId, cancellationToken);
            //  L'utilisateur n'a pas encore de RoadTrip
            if (roadTrip == null)
            {
                roadTrip = new RoadTrip { UserId = userId };
                _dbContext.Roadtrips
                    .Add(roadTrip);
                var step = new Step { CountryCca3 = coountryCca3 };
                step.SetOrder(1);
                roadTrip.Steps.Add(step);
                
            }
            // Le pays existe déjà dans le RoadTrip
            else if (roadTrip.Steps.Any(step => step.CountryCca3 == coountryCca3))
            {
                throw new ArgumentException($"Le pays  '{coountryCca3}' est déja présent dans le Roadtrip.");
            }
            // Le RoadTrip existe mais ne contient aucune étape
            else if (roadTrip.Steps.Count == 0)
            {
                var step = new Step { CountryCca3 = coountryCca3 };
                step.SetOrder(1);
                roadTrip.Steps.Add(step);
            }
            // Le RoadTrip existe et contient déjà des étapes
            else
            {
                int order = roadTrip.Steps.Max(step => step.Order) + 1;
                var step = new Step
                {
                    CountryCca3 = coountryCca3,
                };
                step.SetOrder(order);
                roadTrip.Steps.Add(step);
               
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return roadTrip;


        }
    }
}
