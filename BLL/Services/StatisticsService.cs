using BLL.DTOs.Statistics;
using BLL.Exceptions;
using BLL.Interfaces;
using Common.Enums;
using DAL.Contexts;
using DAL.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class StatisticsService : IStatisticsService
{
    private readonly FitnessTrackerContext _context;
    private readonly IValidator<StatisticsRequestDto> _validator;

    public StatisticsService(
        FitnessTrackerContext context,
        IValidator<StatisticsRequestDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<int> GetWorkoutsCountAsync(
        Guid userId,
        StatisticsRequestDto request, 
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var workoutQuery = _context.Workouts
            .Where(w => w.UserId == userId);

        workoutQuery = ApplyDateFilter(workoutQuery, request);

        if (workoutQuery == null)
        {
            throw new NotFoundException("Not a single workout was found.");
        }

        return workoutQuery.Count();
    }

    public async Task<CaloriesDynamicResponseDto> GetCaloriesDynamicAsync(
         Guid userId,
         StatisticsRequestDto request,
         CancellationToken cancellationToken = default)
    {
        var query = _context.Workouts
            .Where(w => w.UserId == userId);

        query = ApplyDateFilter(query, request);

        var dailyCalories = await query
            .GroupBy(w => w.Date.Date)
            .Select(g => new CaloriesDynamicDto
            {
                Date = g.Key,
                Calories = g.Sum(w => w.Calories)
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        var response = new CaloriesDynamicResponseDto
        {
            DailyCalories = dailyCalories,
            TotalCalories = dailyCalories.Sum(d => d.Calories),
            AverageCaloriesPerDay = dailyCalories.Any()
                ? dailyCalories.Average(d => d.Calories)
                : 0
        };

        return response;
    }

    public async Task<List<WorkoutTypeCountResponseDto>> GetWorkoutTypeStatsAsync(
        Guid userId,
        StatisticsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Workouts
            .Where(w => w.UserId == userId);

        query = ApplyDateFilter(query, request);

        var workouts = await query
            .GroupBy(w => w.Type)
            .Select(g => new WorkoutTypeCountResponseDto
            {
                Type = g.Key,
                TotalCount = g.Count()
            })
            .OrderBy(w => w.Type)
            .ToListAsync(cancellationToken);

        var allTypes = Enum.GetValues(typeof(WorkoutType)).Cast<WorkoutType>();
        var result = allTypes.Select(t => new WorkoutTypeCountResponseDto
        {
            Type = t,
            TotalCount = workouts.FirstOrDefault(w => w.Type == t)?.TotalCount ?? 0
        }).ToList();

        return result;
    }

    public async Task<StatisticsResponseDto> GetStatisticsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var workoutsQuery = _context.Workouts
            .Where(w => w.UserId == userId);

        var totalWorkouts = await workoutsQuery.CountAsync(cancellationToken);
        var averageWorkoutDuration = (int)await workoutsQuery.AverageAsync(w => w.Duration, cancellationToken);
        var longestDistance = await workoutsQuery.MaxAsync(w => w.Distance, cancellationToken);
        var mostCaloriesBurned = await workoutsQuery.MaxAsync(w => w.Calories, cancellationToken);

        var mostFrequentWorkoutType = await workoutsQuery
            .GroupBy(w => w.Type)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken);

        return new StatisticsResponseDto
        {
            TotalWorkouts = totalWorkouts,
            AverageWorkoutDuration = averageWorkoutDuration,
            LongestDistance = longestDistance,
            MostCaloriesBurned = mostCaloriesBurned,
            MostFrequentWorkoutType = mostFrequentWorkoutType
        };
    }

    private IQueryable<Workout> ApplyDateFilter(
        IQueryable<Workout> query,
        StatisticsRequestDto request)
    {
        if (request.StartDate.HasValue)
        {
            query = query.Where(w => w.Date >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(w => w.Date <= request.EndDate.Value);
        }

        return query;
    }
}