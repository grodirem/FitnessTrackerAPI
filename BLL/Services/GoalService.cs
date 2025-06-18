using AutoMapper;
using BLL.DTOs.Goal;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace BLL.Services;

public class GoalService : IGoalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GoalService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<GoalSetDto> _goalValidator;

    public GoalService(
        IUnitOfWork unitOfWork,
        ILogger<GoalService> logger,
        IMapper mapper,
        IValidator<GoalSetDto> goalValidator)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _goalValidator = goalValidator;
    }

    public async Task<GoalResponseDto> SetGoalAsync(Guid userId, GoalSetDto goalDto)
    {
        await _goalValidator.ValidateAsync(goalDto);

        var existingGoal = await _unitOfWork.Goals.GetUserActiveGoalAsync(userId);

        if (existingGoal != null)
        {
            _logger.LogWarning("User {UserId} already has active goal {GoalId}", userId, existingGoal.Id);
            throw new ConflictException("User already has an active goal");
        }

        var newGoal = _mapper.Map<Goal>(goalDto);
        newGoal.UserId = userId;
        newGoal.Active = true;

        await _unitOfWork.Goals.CreateAndSaveAsync(newGoal);

        _logger.LogInformation("Created new goal {GoalId} for user {UserId}",
            newGoal.Id, userId);

        return _mapper.Map<GoalResponseDto>(newGoal);
    }

    public async Task<GoalResponseDto?> GetCurrentGoalAsync(Guid userId)
    {
        var goal = await _unitOfWork.Goals.GetUserActiveGoalAsync(userId);

        _logger.LogInformation(goal == null
            ? "No active goal found for user {UserId}"
            : "Retrieved active goal {GoalId} for user {UserId}",
            userId, goal?.Id);

        return _mapper.Map<GoalResponseDto>(goal);
    }

    public async Task DeactivateGoalAsync(Guid goalId, Guid userId)
    {
        var goal = await _unitOfWork.Goals
            .FindByCondition(g => g.Id == goalId && g.UserId == userId, true)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException($"Goal {goalId} not found for user {userId}");

        if (!goal.Active)
        {
            _logger.LogWarning("Goal {GoalId} is already inactive", goalId);
            return;
        }

        goal.Active = false;
        await _unitOfWork.Goals.UpdateAndSaveAsync(goal);

        _logger.LogInformation("Deactivated goal {GoalId} for user {UserId}",
            goalId, userId);
    }
}