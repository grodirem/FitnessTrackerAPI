using BLL.DTOs.Goal;

namespace BLL.Interfaces;

public interface IGoalService
{
    Task<GoalResponseDto> SetGoalAsync(Guid userId, GoalSetDto goalDto);
    Task<GoalResponseDto> GetCurrentGoalAsync(Guid userId);
    Task DeactivateGoalAsync(Guid goalId, Guid userId);
}
