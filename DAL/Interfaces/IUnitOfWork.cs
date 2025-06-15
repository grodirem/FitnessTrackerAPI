namespace DAL.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IWorkoutRepository Workouts { get; }
    IGoalRepository Goals { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}