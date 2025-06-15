using DAL.Entities;

namespace DAL.Interfaces;

public interface IGoalRepository : IRepositoryBase<Goal>
{
    Task<Goal?> GetUserActiveGoalAsync(Guid userId, bool trackChanges = false);
}