using DAL.Contexts;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class WorkoutRepository : RepositoryBase<Workout>, IWorkoutRepository
{
    public WorkoutRepository(FitnessTrackerContext context) : base(context) { }

    public IQueryable<Workout> AsQueryable() => _context.Workouts;

    public async Task<IEnumerable<Workout>> GetWorkoutsByUserAsync(
        Guid userId,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var workouts = await FindByConditionAsync(w => w.UserId == userId, trackChanges, cancellationToken);
        return workouts.OrderByDescending(w => w.Date);
    }

    public async Task<Workout?> GetWorkoutByIdAsync(
        Guid id,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        return await FindFirstByConditionAsync(w => w.Id == id, trackChanges, cancellationToken);
    }
}