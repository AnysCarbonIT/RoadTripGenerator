using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoadTripGenerator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace RoadTripGenerator.Infrastructure.Persistence.Configurations
{
    public class RoadTripConfiguration : IEntityTypeConfiguration<RoadTrip>
    {
        public void Configure(EntityTypeBuilder<RoadTrip> builder)
        {
            builder.ToTable("RoadTrips");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.UserId)
                   .IsRequired();
            // un userid est unique pour chaque roadtrip
            builder.HasIndex(r => r.UserId)
                   .IsUnique();

            // -> pas necessaire de configurer la relation avec Steps car elle est déjà configurée dans la classe StepConfiguration
            //builder.HasMany(r => r.Steps)
            //       .WithOne(s => s.Roadtrip)
            //       .HasForeignKey(s => s.RoadtripId)
            //       .IsRequired();
        }
    }
}
