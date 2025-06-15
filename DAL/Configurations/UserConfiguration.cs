using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.BirthDate)
                .IsRequired();

            builder.Property(u => u.Gender)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}