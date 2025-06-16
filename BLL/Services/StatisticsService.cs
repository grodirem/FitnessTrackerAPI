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

    public async Task<StatisticsResponseDto> GetUserStatisticsAsync(Guid userId, StatisticsRequestDto requestDto)
    {
        await _validator.ValidateAndThrowAsync(requestDto);

        var workoutsQuery = _context.Workouts
            .Where(w => w.UserId == userId)
            .AsQueryable();

        workoutsQuery = ApplyDateFilter(workoutsQuery, requestDto);

        var workouts = await workoutsQuery.ToListAsync();

        if (!workouts.Any())
        {
            throw new NotFoundException("No workouts found for the specified period");
        }

        return new StatisticsResponseDto
        {
            TotalWorkouts = workouts.Count,
            TotalCaloriesBurned = workouts.Sum(w => w.Calories),
            AverageWorkoutDuration = (int)workouts.Average(w => w.Duration),
            FavoriteWorkoutType = workouts
                .GroupBy(w => w.Type)
                .OrderByDescending(g => g.Count())
                .First().Key,
            WorkoutsByPeriod = GroupWorkoutsByPeriod(workouts, requestDto.PeriodType),
            WorkoutsByType = workouts
                .GroupBy(w => w.Type)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<StatisticsResponseDto> GetWorkoutTypeStatisticsAsync(Guid userId, StatisticsRequestDto requestDto)
    {
        await _validator.ValidateAndThrowAsync(requestDto);

        var workoutsQuery = _context.Workouts
            .Where(w => w.UserId == userId)
            .AsQueryable();

        workoutsQuery = ApplyDateFilter(workoutsQuery, requestDto);

        var workouts = await workoutsQuery.ToListAsync();

        if (!workouts.Any())
        {
            throw new NotFoundException("No workouts found for the specified period");
        }

        return new StatisticsResponseDto
        {
            TotalWorkouts = workouts.Count,
            TotalCaloriesBurned = workouts.Sum(w => w.Calories),
            AverageWorkoutDuration = (int)workouts.Average(w => w.Duration),
            WorkoutsByType = workouts
                .GroupBy(w => w.Type)
                .ToDictionary(g => g.Key, g => g.Count()),
            WorkoutsByPeriod = GroupWorkoutsByTypeAndPeriod(workouts, requestDto.PeriodType)
        };
    }

    private IQueryable<Workout> ApplyDateFilter(IQueryable<Workout> query, StatisticsRequestDto requestDto)
    {
        if (requestDto.StartDate.HasValue)
        {
            query = query.Where(w => w.Date >= requestDto.StartDate.Value);
        }

        if (requestDto.EndDate.HasValue)
        {
            query = query.Where(w => w.Date <= requestDto.EndDate.Value);
        }

        return query;
    }

    private Dictionary<string, int> GroupWorkoutsByPeriod(List<Workout> workouts, PeriodType periodType)
    {
        return periodType switch
        {
            PeriodType.Day => workouts
                .GroupBy(w => w.Date.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.Count()),

            PeriodType.Week => workouts
                .GroupBy(w => $"{w.Date.Year}-W{GetWeekNumber(w.Date)}")
                .ToDictionary(g => g.Key, g => g.Count()),

            PeriodType.Month => workouts
                .GroupBy(w => w.Date.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Count()),

            PeriodType.Year => workouts
                .GroupBy(w => w.Date.Year.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),

            _ => throw new ArgumentException("Invalid period type")
        };
    }

    private Dictionary<string, int> GroupWorkoutsByTypeAndPeriod(List<Workout> workouts, PeriodType periodType)
    {
        var result = new Dictionary<string, int>();

        foreach (var workout in workouts)
        {
            var periodKey = periodType switch
            {
                PeriodType.Day => $"{workout.Type}_{workout.Date:yyyy-MM-dd}",
                PeriodType.Week => $"{workout.Type}_{workout.Date.Year}-W{GetWeekNumber(workout.Date)}",
                PeriodType.Month => $"{workout.Type}_{workout.Date:yyyy-MM}",
                PeriodType.Year => $"{workout.Type}_{workout.Date.Year}",
                _ => throw new ArgumentException("Invalid period type")
            };

            if (result.ContainsKey(periodKey))
            {
                result[periodKey]++;
            }
            else
            {
                result[periodKey] = 1;
            }
        }

        return result;
    }

    private static int GetWeekNumber(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        return culture.Calendar.GetWeekOfYear(
            date,
            culture.DateTimeFormat.CalendarWeekRule,
            culture.DateTimeFormat.FirstDayOfWeek);
    }
}