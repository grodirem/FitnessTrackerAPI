using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Contexts;

public class FitnessTrackerContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public FitnessTrackerContext(DbContextOptions<FitnessTrackerContext> options)
        : base(options) { }

    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessTrackerContext).Assembly);
    }
}