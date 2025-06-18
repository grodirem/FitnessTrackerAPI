using DAL.Contexts;
using DAL.Interfaces;

namespace DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FitnessTrackerContext _context;
    private IWorkoutRepository? _workoutRepository;
    private IGoalRepository? _goalRepository;

    public UnitOfWork(FitnessTrackerContext context, IWorkoutRepository workoutRepository, IGoalRepository goalRepository)
    {
        _context = context;
        _goalRepository = goalRepository;
        _workoutRepository = workoutRepository;
    }

    public IWorkoutRepository Workouts =>
        _workoutRepository ??= new WorkoutRepository(_context);

    public IGoalRepository Goals =>
        _goalRepository ??= new GoalRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}