using DAL.Entities;

namespace DAL.Interfaces;

public interface IWorkoutRepository : IRepositoryBase<Workout>
{
    IQueryable<Workout> AsQueryable();

    Task<IEnumerable<Workout>> GetWorkoutsByUserAsync(
        Guid userId, 
        bool trackChanges = false, 
        CancellationToken cancellationToken = default);

    Task<Workout?> GetWorkoutByIdAsync(
        Guid id, 
        bool trackChanges = false, 
        CancellationToken cancellationToken = default);
}