using AutoMapper;
using BLL.DTOs.Integration;
using BLL.DTOs.Workout;
using BLL.Exceptions;
using BLL.Extensions;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<WorkoutCreateDto> _createValidator;
    private readonly IValidator<WorkoutUpdateDto> _updateValidator;
    private readonly IValidator<WorkoutFilterDto> _filterValidator;

    public WorkoutService(
        IWorkoutRepository workoutRepository,
        IMapper mapper,
        IValidator<WorkoutCreateDto> createValidator,
        IValidator<WorkoutUpdateDto> updateValidator,
        IValidator<WorkoutFilterDto> filterValidator)
    {
        _workoutRepository = workoutRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _filterValidator = filterValidator;
    }

    public async Task<WorkoutResponseDto> CreateWorkoutAsync(
        Guid userId,
        WorkoutCreateDto createDto,
        CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(createDto, cancellationToken);

        var workout = _mapper.Map<Workout>(createDto);
        workout.UserId = userId;

        _workoutRepository.Create(workout);
        await _workoutRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task<WorkoutResponseDto> GetWorkoutAsync(
        Guid workoutId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var workout = await _workoutRepository.FindFirstByConditionAsync(
            w => w.Id == workoutId && w.UserId == userId, 
            cancellationToken: cancellationToken);

        if (workout == null)
        {
            throw new NotFoundException("Workout not found");
        }

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetUserWorkoutsAsync(
        Guid userId,
        WorkoutFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        await _filterValidator.ValidateAndThrowAsync(filterDto, cancellationToken);

        var query = _workoutRepository.AsQueryable()
            .Where(w => w.UserId == userId)
            .WhereIf(filterDto.Type.HasValue, w => w.Type == filterDto.Type.GetValueOrDefault())
            .WhereIf(filterDto.FromDate.HasValue, w => w.Date >= filterDto.FromDate.GetValueOrDefault())
            .WhereIf(filterDto.ToDate.HasValue, w => w.Date <= filterDto.ToDate.GetValueOrDefault())
            .WhereIf(filterDto.MinDuration.HasValue, w => w.Duration >= filterDto.MinDuration.GetValueOrDefault())
            .WhereIf(filterDto.MaxDuration.HasValue, w => w.Duration <= filterDto.MaxDuration.GetValueOrDefault())
            .WhereIf(filterDto.MinCalories.HasValue, w => w.Calories >= filterDto.MinCalories.GetValueOrDefault())
            .WhereIf(filterDto.MaxCalories.HasValue, w => w.Calories <= filterDto.MaxCalories.GetValueOrDefault())
            .OrderByField(filterDto.SortBy, filterDto.SortDescending ?? false);

        var workouts = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WorkoutResponseDto>>(workouts);
    }

    public async Task UpdateWorkoutAsync(
        Guid workoutId,
        Guid userId,
        WorkoutUpdateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(updateDto, cancellationToken);

        var workout = await _workoutRepository.FindFirstByConditionAsync(
            w => w.Id == workoutId && w.UserId == userId,
            trackChanges: true,
            cancellationToken);

        if (workout == null)
        {
            throw new NotFoundException("Workout not found");
        }

        _mapper.Map(updateDto, workout);
        await _workoutRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWorkoutAsync(
        Guid workoutId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var workout = await _workoutRepository.FindFirstByConditionAsync(
            w => w.Id == workoutId && w.UserId == userId, 
            cancellationToken: cancellationToken);

        if (workout == null)
        {
            throw new NotFoundException("Workout not found");
        }

        _workoutRepository.Delete(workout);
        await _workoutRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkoutResponseDto> CreateWorkoutFromIntegrationAsync(
        Guid userId,
        ExternalWorkoutDto externalWorkoutDto,
        CancellationToken cancellationToken = default)
    {
        var workout = _mapper.Map<Workout>(externalWorkoutDto);
        workout.UserId = userId;

        _workoutRepository.Create(workout);
        await _workoutRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkoutResponseDto>(workout);
    }
}