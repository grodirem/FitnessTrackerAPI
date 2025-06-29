using DAL.Contexts;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories;

public class GoalRepository : RepositoryBase<Goal>, IGoalRepository
{
    public GoalRepository(FitnessTrackerContext context) : base(context) { }

    public async Task<Goal?> GetUserActiveGoalAsync(
        Guid userId, 
        bool trackChanges = false, 
        CancellationToken cancellationToken = default)
    {
        return await FindFirstByConditionAsync(
            g => g.UserId == userId && g.Active, 
            cancellationToken : cancellationToken);
    }
}