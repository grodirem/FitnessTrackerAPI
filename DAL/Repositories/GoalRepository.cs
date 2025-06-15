using DAL.Contexts;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class GoalRepository : RepositoryBase<Goal>, IGoalRepository
{
    public GoalRepository(FitnessTrackerContext context) : base(context) { }

    public async Task<Goal?> GetUserActiveGoalAsync(Guid userId, bool trackChanges = false) =>
        await FindByCondition(g => g.UserId == userId && g.Active, trackChanges)
            .FirstOrDefaultAsync();
}