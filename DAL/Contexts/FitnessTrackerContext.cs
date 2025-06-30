using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contexts;

public class FitnessTrackerContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public FitnessTrackerContext(DbContextOptions<FitnessTrackerContext> options)
        : base(options) { }

    public required DbSet<Workout> Workouts { get; set; }
    public required DbSet<Goal> Goals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessTrackerContext).Assembly);
    }
}