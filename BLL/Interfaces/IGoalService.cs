using BLL.DTOs.Goal;

namespace BLL.Interfaces;

public interface IGoalService
{
    Task<GoalResponseDto> SetGoalAsync(Guid userId, GoalSetDto goalDto, CancellationToken cancellationToken = default);
    Task<GoalResponseDto> GetCurrentGoalAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeactivateGoalAsync(Guid goalId, Guid userId, CancellationToken cancellationToken = default);
}
