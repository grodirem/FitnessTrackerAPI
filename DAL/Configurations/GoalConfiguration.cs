using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.DAL.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.TargetWorkoutsPerWeek)
            .IsRequired();

        builder.Property(g => g.TargetCaloriesPerWorkout)
            .IsRequired();

        builder.Property(g => g.Active)
            .HasDefaultValue(true);

        builder.HasOne(g => g.User)
            .WithMany(u => u.Goals)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}