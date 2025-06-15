using DAL.Entities;

namespace DAL.Interfaces;

public interface IWorkoutRepository : IRepositoryBase<Workout>
{
    Task<IEnumerable<Workout>> GetWorkoutsByUserAsync(Guid userId, bool trackChanges = false);
    Task<Workout?> GetWorkoutByIdAsync(Guid id, bool trackChanges = false);
}