using DAL.Contexts;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class GoalRepositoryTests : IDisposable
{
    private readonly FitnessTrackerContext _context;
    private readonly GoalRepository _repository;

    public GoalRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FitnessTrackerContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FitnessTrackerContext(options);
        _repository = new GoalRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Name = "Test User"
        };

        _context.Users.Add(user);

        _context.Goals.AddRange(
            new Goal
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                Active = false
            },
            new Goal
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                Active = true
            }
        );

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserActiveGoalAsync_ShouldReturn_ActiveGoal()
    {
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var result = await _repository.GetUserActiveGoalAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
        Assert.True(result.Active);
    }

    [Fact]
    public async Task GetUserActiveGoalAsync_ShouldReturn_NullIfNoActiveGoalExists()
    {
        var newUser = new User
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Name = "User Without Active Goal"
        };

        _context.Users.Add(newUser);
        _context.Goals.Add(new Goal
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            User = newUser,
            Active = false
        });
        _context.SaveChanges();

        var result = await _repository.GetUserActiveGoalAsync(newUser.Id);

        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
