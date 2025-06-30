using AutoMapper;
using BLL.DTOs.Goal;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;
    private readonly ILogger<GoalService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<GoalSetDto> _goalValidator;

    public GoalService(
        IGoalRepository goalRepository,
        ILogger<GoalService> logger,
        IMapper mapper,
        IValidator<GoalSetDto> goalValidator)
    {
        _goalRepository = goalRepository;
        _logger = logger;
        _mapper = mapper;
        _goalValidator = goalValidator;
    }

    public async Task<GoalResponseDto> SetGoalAsync(
        Guid userId, 
        GoalSetDto goalDto, 
        CancellationToken cancellationToken = default)
    {
        await _goalValidator.ValidateAsync(goalDto, cancellationToken);

        var existingGoal = await _goalRepository.GetUserActiveGoalAsync(userId, cancellationToken: cancellationToken);
        if (existingGoal != null)
        {
            throw new ConflictException("User already has an active goal");
        }

        var goal = _mapper.Map<Goal>(goalDto);
        goal.UserId = userId;
        goal.Active = true;

        _goalRepository.Create(goal);
        await _goalRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GoalResponseDto>(goal);
    }

    public async Task<GoalResponseDto?> GetCurrentGoalAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var goal = await _goalRepository.GetUserActiveGoalAsync(userId, cancellationToken: cancellationToken);
        return _mapper.Map<GoalResponseDto>(goal);
    }

    public async Task DeactivateGoalAsync(
        Guid goalId, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var goal = await _goalRepository.FindFirstByConditionAsync(
            g => g.Id == goalId && g.UserId == userId,
            true,
            cancellationToken);

        if (goal == null)
        {
            throw new NotFoundException($"Goal {goalId} not found for user {userId}");
        }

        if (!goal.Active) { return; }

        goal.Active = false;
        await _goalRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deactivated goal {GoalId} for user {UserId}",
            goalId, userId);
    }
}