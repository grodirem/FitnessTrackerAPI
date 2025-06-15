using DAL.Contexts;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class WorkoutRepository : RepositoryBase<Workout>, IWorkoutRepository
{
    public WorkoutRepository(FitnessTrackerContext context) : base(context) { }

    public async Task<IEnumerable<Workout>> GetWorkoutsByUserAsync(Guid userId, bool trackChanges = false) =>
        await FindByCondition(w => w.UserId == userId, trackChanges)
            .OrderByDescending(w => w.Date)
            .ToListAsync();

    public async Task<Workout?> GetWorkoutByIdAsync(Guid id, bool trackChanges = false) =>
        await FindByCondition(w => w.Id == id, trackChanges)
            .SingleOrDefaultAsync();
}