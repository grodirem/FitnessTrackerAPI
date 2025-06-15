using AutoMapper;
using BLL.DTOs.Workout;
using Common.Enums;
using DAL.Contexts;
using BLL.Interfaces;
using System.Data.Entity;

namespace BLL.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly FitnessTrackerContext _context;
        private readonly IMapper _mapper;
        private readonly IWorkoutService _workoutService;
        private readonly Random _random = new Random();

        public IntegrationService(
            FitnessTrackerContext context,
            IMapper mapper,
            IWorkoutService workoutService)
        {
            _context = context;
            _mapper = mapper;
            _workoutService = workoutService;
        }

        public async Task ImportWorkoutsFromExternalServiceAsync(Guid userId, IntegrationSourceType serviceType)
        {
            var mockWorkouts = GenerateMockWorkouts(userId, serviceType);

            foreach (var workoutDto in mockWorkouts)
            {
                try
                {
                    await _workoutService.CreateWorkoutAsync(userId, workoutDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error importing workout: {ex.Message}");
                }
            }
        }

        public async Task SyncWithExternalServiceAsync(Guid userId, IntegrationSourceType serviceType)
        {
            var localWorkouts = await _context.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Take(10)
                .ToListAsync();

            var externalWorkouts = GenerateMockWorkouts(userId, serviceType);

            foreach (var externalWorkout in externalWorkouts)
            {
                var exists = localWorkouts.Any(w =>
                    Math.Abs((w.Date - externalWorkout.Date).TotalMinutes) < 5 &&
                    w.Type == externalWorkout.Type);

                if (!exists)
                {
                    await _workoutService.CreateWorkoutAsync(userId, externalWorkout);
                }
            }
        }

        private List<WorkoutCreateDto> GenerateMockWorkouts(Guid userId, IntegrationSourceType serviceType)
        {
            var mockWorkouts = new List<WorkoutCreateDto>();
            var workoutTypes = Enum.GetValues(typeof(WorkoutType));
            var daysToGenerate = 30;

            for (int i = 0; i < 15; i++)
            {
                var workoutType = (WorkoutType)workoutTypes.GetValue(_random.Next(workoutTypes.Length));
                var daysAgo = _random.Next(1, daysToGenerate);
                var workoutDate = DateTime.UtcNow.AddDays(-daysAgo);

                mockWorkouts.Add(new WorkoutCreateDto
                {
                    Type = workoutType,
                    Duration = _random.Next(10, 120),
                    Calories = _random.Next(100, 800),
                    Distance = workoutType == WorkoutType.Running || workoutType == WorkoutType.Cycling
                        ? _random.Next(1, 20)
                        : null,
                    Date = workoutDate,
                    Notes = $"Imported from {serviceType} on {DateTime.UtcNow:yyyy-MM-dd}"
                });
            }

            if (serviceType == IntegrationSourceType.GoogleFit)
            {
                mockWorkouts.Add(new WorkoutCreateDto
                {
                    Type = WorkoutType.Walking,
                    Duration = 45,
                    Calories = 250,
                    Distance = 3.5,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Notes = "Google Fit automatic tracking"
                });
            }
            else if (serviceType == IntegrationSourceType.AppleHealth)
            {
                mockWorkouts.Add(new WorkoutCreateDto
                {
                    Type = WorkoutType.Running,
                    Duration = 30,
                    Calories = 300,
                    Distance = 5.0,
                    Date = DateTime.UtcNow.AddDays(-2),
                    Notes = "Apple Health recorded workout"
                });
            }

            return mockWorkouts;
        }
    }
}
