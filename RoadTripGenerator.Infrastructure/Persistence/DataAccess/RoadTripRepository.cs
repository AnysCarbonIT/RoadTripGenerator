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
        public async Task<RoadTrip> DeleteCountryAsync(string userId, string countryCca3, CancellationToken cancellationToken)
        {
            var roadTrip = await _dbContext.Roadtrips
                .AsNoTracking() //utile car on ne va pas chnager les entités dans le contexte, on va juste les lire et contitué nouvelle liste ( replaceasync fera la mise à jour)
                .Include(roadTrip => roadTrip.Steps)
                .FirstOrDefaultAsync(roadTrip => roadTrip.UserId == userId, cancellationToken);
                
            if (roadTrip == null)
            {
                throw new ArgumentException($"L'utilisateur '{userId}' n'a pas de Roadtrip.");
            }
            var stepToRemove = roadTrip.Steps.FirstOrDefault(step => step.CountryCca3 == countryCca3);
            if (stepToRemove == null)
            {
                throw new ArgumentException($"Le pays '{countryCca3}' n'est pas présent dans le Roadtrip de l'utilisateur '{userId}'.");
            }
            //compose une nouvelle liste sans l'element a supprimer puis reultise replaceAsync ( du put) 
            var stepsWithoutDeleted = roadTrip.Steps
                .Where(step => step.CountryCca3 != countryCca3)
                .OrderBy(step => step.Order)
                .ToList();
            var newStepsList = new List<Step>();
            int order = 1;
            foreach (var step in stepsWithoutDeleted)
            {
                var newStep = new Step
                {
                    CountryCca3 = step.CountryCca3,
                };
                newStep.SetOrder(order); 
                newStepsList.Add(newStep);
                order++;
            }
            return await ReplaceAsync(userId, newStepsList, cancellationToken);
        }
    }
}
