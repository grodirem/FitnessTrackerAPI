using AutoMapper;
using BLL.DTOs.Goal;
using BLL.Exceptions;
using BLL.Services;
using DAL.Entities;
using DAL.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace BLL.Tests;

public class GoalServiceTests
{
    private readonly Mock<IGoalRepository> _goalRepositoryMock;
    private readonly Mock<ILogger<GoalService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<GoalSetDto>> _validatorMock;

    private readonly GoalService _goalService;

    public GoalServiceTests()
    {
        _goalRepositoryMock = new Mock<IGoalRepository>();
        _loggerMock = new Mock<ILogger<GoalService>>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<GoalSetDto>>();

        _goalService = new GoalService(
            _goalRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task SetGoalAsync_ShouldCreateGoal_WhenNoActiveGoal()
    {
        var userId = Guid.NewGuid();
        var dto = new GoalSetDto();
        var goal = new Goal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = new User { Id = userId, Name = "Test User" }
        };

        var responseDto = new GoalResponseDto();

        _validatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _goalRepositoryMock
            .Setup(r => r.GetUserActiveGoalAsync(userId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Goal?)null);

        _mapperMock.Setup(m => m.Map<Goal>(dto)).Returns(goal);
        _mapperMock.Setup(m => m.Map<GoalResponseDto>(goal)).Returns(responseDto);

        var result = await _goalService.SetGoalAsync(userId, dto);

        Assert.NotNull(result);
        Assert.Equal(responseDto, result);
        _goalRepositoryMock.Verify(r => r.Create(goal), Times.Once);
        _goalRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetGoalAsync_ShouldThrowConflict_WhenActiveGoalExists()
    {
        var userId = Guid.NewGuid();
        var dto = new GoalSetDto();

        var goal = new Goal
        {
            UserId = userId,
            User = new User { Id = userId, Name = "Test" }
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _goalRepositoryMock
            .Setup(r => r.GetUserActiveGoalAsync(userId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goal);

        await Assert.ThrowsAsync<ConflictException>(() =>
            _goalService.SetGoalAsync(userId, dto));
    }

    [Fact]
    public async Task GetCurrentGoalAsync_ShouldReturnMappedDto_IfExists()
    {
        var userId = Guid.NewGuid();
        var goal = new Goal
        {
            UserId = userId,
            User = new User { Id = userId, Name = "Test" }
        };

        var response = new GoalResponseDto();

        _goalRepositoryMock
            .Setup(r => r.GetUserActiveGoalAsync(userId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goal);

        _mapperMock
            .Setup(m => m.Map<GoalResponseDto>(goal))
            .Returns(response);

        var result = await _goalService.GetCurrentGoalAsync(userId);

        Assert.Equal(response, result);
    }

    [Fact]
    public async Task DeactivateGoalAsync_ShouldSetInactive_AndSave_WhenGoalActive()
    {
        var goalId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var goal = new Goal
        {
            Id = goalId,
            UserId = userId,
            Active = true,
            User = new User { Id = userId, Name = "Test" }
        };

        Expression<Func<Goal, bool>> predicate =
            g => g.Id == goalId && g.UserId == userId;

        _goalRepositoryMock
            .Setup(r => r.FindFirstByConditionAsync(It.IsAny<Expression<Func<Goal, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goal);

        await _goalService.DeactivateGoalAsync(goalId, userId);

        Assert.False(goal.Active);
        _goalRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivateGoalAsync_ShouldThrowNotFound_IfGoalMissing()
    {
        var goalId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _goalRepositoryMock
            .Setup(r => r.FindFirstByConditionAsync(It.IsAny<Expression<Func<Goal, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Goal?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _goalService.DeactivateGoalAsync(goalId, userId));
    }

    [Fact]
    public async Task DeactivateGoalAsync_ShouldDoNothing_WhenGoalAlreadyInactive()
    {
        var goalId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var goal = new Goal
        {
            Id = goalId,
            UserId = userId,
            Active = false,
            User = new User { Id = userId, Name = "Test" }
        };

        _goalRepositoryMock
            .Setup(r => r.FindFirstByConditionAsync(It.IsAny<Expression<Func<Goal, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goal);

        await _goalService.DeactivateGoalAsync(goalId, userId);

        _goalRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
