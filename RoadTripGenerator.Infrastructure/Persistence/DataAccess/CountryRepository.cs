using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Domain.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Infrastructure.Persistence.DataAccess
{
    public class CountryRepository :ICountryRepository
    {
        public readonly RoadTripGeneratorDbContext _dbContext;
        public  CountryRepository(RoadTripGeneratorDbContext dbContext)
        {
            _dbContext = dbContext;
            
        }
        public async Task<IReadOnlyList<Country>> GetCountriesAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .OrderBy(country => country.Cca3)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasCountriesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Countries.AnyAsync(cancellationToken);
        }

        public async Task AddCountryAsync(IReadOnlyList<Country> countries, CancellationToken cancellationToken)
        {
                await _dbContext.Countries.AddRangeAsync(countries, cancellationToken); // AddRange ajoute plusoeiru lignes  ici liste de country
                await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
