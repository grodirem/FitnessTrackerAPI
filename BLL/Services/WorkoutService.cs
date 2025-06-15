using AutoMapper;
using BLL.DTOs.Workout;
using BLL.Exceptions;
using BLL.Extensions;
using BLL.Interfaces;
using DAL.Contexts;
using DAL.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class WorkoutService : IWorkoutService
{
    private readonly FitnessTrackerContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<WorkoutCreateDto> _createValidator;
    private readonly IValidator<WorkoutUpdateDto> _updateValidator;
    private readonly IValidator<WorkoutFilterDto> _filterValidator;

    public WorkoutService(
        FitnessTrackerContext context,
        IMapper mapper,
        IValidator<WorkoutCreateDto> createValidator,
        IValidator<WorkoutUpdateDto> updateValidator,
        IValidator<WorkoutFilterDto> filterValidator)
    {
        _context = context;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _filterValidator = filterValidator;
    }

    public async Task<WorkoutResponseDto> CreateWorkoutAsync(Guid userId, WorkoutCreateDto createDto)
    {
        await _createValidator.ValidateAndThrowAsync(createDto);

        var workout = _mapper.Map<Workout>(createDto);
        workout.UserId = userId;

        await _context.Workouts.AddAsync(workout);
        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task<WorkoutResponseDto> GetWorkoutAsync(Guid workoutId, Guid userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

        if (workout == null)
        {
            throw new NotFoundException("Workout not found");
        }

        return _mapper.Map<WorkoutResponseDto>(workout);
    }

    public async Task<IEnumerable<WorkoutResponseDto>> GetUserWorkoutsAsync(Guid userId, WorkoutFilterDto filterDto)
    {
        await _filterValidator.ValidateAndThrowAsync(filterDto);

        var query = _context.Workouts
            .Where(w => w.UserId == userId)
            .WhereIf(filterDto.Type.HasValue, w => w.Type == filterDto.Type.Value)
            .WhereIf(filterDto.FromDate.HasValue, w => w.Date >= filterDto.FromDate.Value)
            .WhereIf(filterDto.ToDate.HasValue, w => w.Date <= filterDto.ToDate.Value)
            .WhereIf(filterDto.MinDuration.HasValue, w => w.Duration >= filterDto.MinDuration.Value)
            .WhereIf(filterDto.MaxDuration.HasValue, w => w.Duration <= filterDto.MaxDuration.Value)
            .WhereIf(filterDto.MinCalories.HasValue, w => w.Calories >= filterDto.MinCalories.Value)
            .WhereIf(filterDto.MaxCalories.HasValue, w => w.Calories <= filterDto.MaxCalories.Value)
            .OrderByField(filterDto.SortBy?.ToLower(), filterDto.SortDescending);

        var workouts = await query.ToListAsync();
        return _mapper.Map<IEnumerable<WorkoutResponseDto>>(workouts);
    }

    public async Task UpdateWorkoutAsync(Guid workoutId, Guid userId, WorkoutUpdateDto updateDto)
    {
        await _updateValidator.ValidateAndThrowAsync(updateDto);

        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

        if (workout == null)
        {
            throw new NotFoundException("Workout not found");
        }

        _mapper.Map(updateDto, workout);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteWorkoutAsync(Guid workoutId, Guid userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

        if (workout == null)
        {
            throw new NotFoundException("Workout not found");
        }

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();
    }
}