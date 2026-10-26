using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoadTripGenerator.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace RoadTripGenerator.Infrastructure.Persistence.Configurations
{
    internal class StepConfiguration : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {   
            builder.ToTable("Steps");

            builder.HasKey(s => s.Id);             
            builder.Property(s => s.RoadTripId)
                   .IsRequired();
            builder.Property(s => s.CountryCca3)
                   .HasMaxLength(3)
                   .IsRequired();

            builder.Property(s => s.Order);

            builder.HasOne(s => s.Roadtrip)
                   .WithMany(r => r.Steps)
                   .HasForeignKey(s => s.RoadTripId)
                   .IsRequired();
            // Un pays ne peut apparaître qu'une fois dans un itinéraire.
            builder.HasIndex(s => new { s.RoadTripId, s.CountryCca3 })
                   .IsUnique();

            builder.HasOne(s=> s.Country)
                   .WithMany()
                   .HasForeignKey(s => s.CountryCca3)
                   .OnDelete(DeleteBehavior.Restrict) // Empêche la suppression d'un pays si d'autre étapes y font référence
                   .IsRequired();
        }
    }
}
