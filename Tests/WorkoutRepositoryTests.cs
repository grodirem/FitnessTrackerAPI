using DAL.Contexts;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DAL.Tests;

public class WorkoutRepositoryTests : IDisposable
{
    private readonly FitnessTrackerContext _context;
    private readonly WorkoutRepository _repository;

    public WorkoutRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FitnessTrackerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FitnessTrackerContext(options);
        _repository = new WorkoutRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user1 = new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "u1" };
        var user2 = new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "u2" };

        var workouts = new List<Workout>
        {
            new Workout
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                User = user1,
                Date = DateTime.Now.AddDays(-2)
            },
            new Workout
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                User = user1,
                Date = DateTime.Now.AddDays(-1)
            },
            new Workout
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                User = user2,
                Date = DateTime.Now
            }
        };

        _context.Users.AddRange(user1, user2);
        _context.Workouts.AddRange(workouts);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetWorkoutsByUserAsync_ReturnsWorkoutsForSpecificUser()
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var result = await _repository.GetWorkoutsByUserAsync(userId);

        Assert.Equal(2, result.Count());
        Assert.All(result, w => Assert.Equal(userId, w.UserId));
        Assert.True(result.First().Date > result.Last().Date);
    }

    [Fact]
    public async Task GetWorkoutsByUserAsync_ReturnsEmptyListForNonExistentUser()
    {
        var userId = Guid.NewGuid();

        var result = await _repository.GetWorkoutsByUserAsync(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_ReturnsCorrectWorkout()
    {
        var expectedWorkout = await _context.Workouts.FirstAsync();

        var result = await _repository.GetWorkoutByIdAsync(expectedWorkout.Id);

        Assert.NotNull(result);
        Assert.Equal(expectedWorkout.Id, result.Id);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_ReturnsNullForNonExistentId()
    {
        var nonExistentId = Guid.NewGuid();

        var result = await _repository.GetWorkoutByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public void AsQueryable_ReturnsAllWorkouts()
    {
        var result = _repository.AsQueryable();

        Assert.Equal(3, result.Count());
    }
}