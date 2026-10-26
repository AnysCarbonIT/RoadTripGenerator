using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoadTripGenerator.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace RoadTripGenerator.Infrastructure.Persistence.Configurations
{
    internal class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");

            // La vue publique est en lecture seule ; EF persiste la liste privée.
            builder.Ignore(c => c.CapitalCities);
            builder.PrimitiveCollection<List<string>>("_capitalCities")
                   .HasColumnName("CapitalCities")
                   .HasColumnType("text[]")
                   .IsRequired();

            builder.HasKey(c => c.Cca3);

            builder.Property(c => c.Cca3)
                   .HasMaxLength(3)
                   .IsRequired();

            builder.Property(c => c.Name)
                   .IsRequired();

            builder.Property(c => c.Region)
                   .IsRequired();
            builder.Property(c => c.Flag)
                   .IsRequired();
            builder.Property(c => c.Population)
                   .IsRequired();
        }
    }
}
