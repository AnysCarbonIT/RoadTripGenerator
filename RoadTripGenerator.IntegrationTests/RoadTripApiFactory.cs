using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RoadTripGenerator.Domain.Models;
using RoadTripGenerator.Infrastructure.Persistence;

namespace RoadTripGenerator.IntegrationTests
{
    public class RoadTripApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        public async Task InitializeDatabaseAsync()
        {
            string? connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__RoadTripGeneratorDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("La connexion PostgreSQL de test est obligatoire.");
            }

            if (new NpgsqlConnectionStringBuilder(connectionString).Database != "roadtrip_test")
            {
                throw new InvalidOperationException("Les tests doivent utiliser uniquement la base roadtrip_test.");
            }

            var options = new DbContextOptionsBuilder<RoadTripGeneratorDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            await using var dbContext = new RoadTripGeneratorDbContext(options);
            await dbContext.Database.MigrateAsync(); // Applique les migrations si les table de test n'existent pas encore

            if (!await dbContext.Countries.AnyAsync())
            {
                dbContext.Countries.AddRange(
                    new Country
                    {
                        Cca3 = "HRV", Name = "Croatia", Region = "Europe",
                        Population = 3_800_000, CapitalCities = new List<string> { "Zagreb" }, Flag = "🇭🇷"
                    },
                    new Country
                    {
                        Cca3 = "FRA", Name = "France", Region = "Europe",
                        Population = 68_000_000, CapitalCities = new List<string> { "Paris" }, Flag = "🇫🇷"
                    },
                    new Country
                    {
                        Cca3 = "DEU", Name = "Germany", Region = "Europe",
                        Population = 84_000_000, CapitalCities = new List<string> { "Berlin" }, Flag = "🇩🇪"
                    });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
