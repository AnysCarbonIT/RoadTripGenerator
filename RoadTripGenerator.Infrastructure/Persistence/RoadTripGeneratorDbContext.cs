using Microsoft.EntityFrameworkCore;
using RoadTripGenerator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Infrastructure.Persistence
{
    public class RoadTripGeneratorDbContext : DbContext
    {
        public RoadTripGeneratorDbContext(DbContextOptions<RoadTripGeneratorDbContext> options): base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(RoadTripGeneratorDbContext).Assembly);
}
        public DbSet<Country> Countries { get; set; } = null!;
        //public DbSet<Roadtrip> Roadtrips { get; set; }
    }

    }
