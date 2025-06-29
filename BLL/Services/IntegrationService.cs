using AutoMapper;
using BLL.DTOs.Integration;
using BLL.Interfaces;
using Common.Enums;
using DAL.Contexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BLL.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly FitnessTrackerContext _context;
        private readonly IMapper _mapper;
        private readonly IWorkoutService _workoutService;
        private readonly IValidator<ExternalWorkoutDto> _validator;
        private readonly Random _random = new();

        public IntegrationService(
            FitnessTrackerContext context,
            IMapper mapper,
            IWorkoutService workoutService,
            IValidator<ExternalWorkoutDto> validator)
        {
            _context = context;
            _mapper = mapper;
            _workoutService = workoutService;
            _validator = validator;
        }

        public async Task ImportWorkoutsFromExternalServiceAsync(
            Guid userId,
            IntegrationSourceType serviceType,
            CancellationToken cancellationToken = default)
        {
            var mockWorkouts = GenerateMockWorkouts(serviceType);

            foreach (var workoutDto in mockWorkouts)
            {
                try
                {
                    await _validator.ValidateAndThrowAsync(workoutDto, cancellationToken);
                    await _workoutService.CreateWorkoutFromIntegrationAsync(
                        userId, workoutDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error importing workout: {ex.Message}");
                }
            }
        }

        public async Task SyncWithExternalServiceAsync(
            Guid userId,
            IntegrationSourceType serviceType,
            CancellationToken cancellationToken = default)
        {
            var settings = await GetUserIntegrationSettingsAsync(userId);

            if ((serviceType == IntegrationSourceType.GoogleFit && !settings.GoogleFitEnabled) ||
                (serviceType == IntegrationSourceType.AppleHealth && !settings.AppleHealthEnabled))
            {
                throw new InvalidOperationException($"Integration with {serviceType} is disabled");
            }

            var lastSyncDate = settings.LastSyncDate ?? DateTime.UtcNow.AddDays(-7);
            var newWorkouts = GenerateMockWorkouts(serviceType)
                .Where(w => w.StartTime > lastSyncDate)
                .ToList();

            foreach (var workoutDto in newWorkouts)
            {
                var exists = await CheckIfWorkoutExistsAsync(userId, workoutDto, cancellationToken);
                if (!exists)
                {
                    await _workoutService.CreateWorkoutFromIntegrationAsync(
                        userId, workoutDto, cancellationToken);
                }
            }

            settings.LastSyncDate = DateTime.UtcNow;
            await UpdateUserIntegrationSettingsAsync(userId, settings);
        }

        private async Task<bool> CheckIfWorkoutExistsAsync(
            Guid userId,
            ExternalWorkoutDto workoutDto,
            CancellationToken cancellationToken)
        {
            return await _context.Workouts
                .AnyAsync(w => w.UserId == userId &&
                             w.Type == workoutDto.Type &&
                             Math.Abs((w.Date - workoutDto.StartTime.Date).TotalDays) < 1 &&
                             Math.Abs(w.Duration - workoutDto.Duration) < 10,
                    cancellationToken);
        }

        private List<ExternalWorkoutDto> GenerateMockWorkouts(IntegrationSourceType serviceType)
        {
            var mockWorkouts = new List<ExternalWorkoutDto>();
            var workoutTypes = Enum.GetValues(typeof(WorkoutType));
            var daysToGenerate = 30;

            for (int i = 0; i < 15; i++)
            {
                var workoutType = (WorkoutType)workoutTypes.GetValue(_random.Next(workoutTypes.Length))!;
                var daysAgo = _random.Next(1, daysToGenerate);
                var startTime = DateTime.UtcNow.AddDays(-daysAgo);
                var endTime = startTime.AddMinutes(_random.Next(10, 120));

                mockWorkouts.Add(new ExternalWorkoutDto
                {
                    Type = workoutType,
                    Duration = (int)(endTime - startTime).TotalMinutes,
                    Calories = _random.Next(100, 800),
                    Distance = workoutType == WorkoutType.Running || workoutType == WorkoutType.Cycling
                        ? _random.Next(1, 20)
                        : null,
                    StartTime = startTime,
                    EndTime = endTime,
                    Source = serviceType,
                    Notes = $"Mock data from {serviceType}"
                });
            }

            return mockWorkouts;
        }

        public async Task<IntegrationSettingsDto> GetUserIntegrationSettingsAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.IntegrationSettingsJson == null)
                return new IntegrationSettingsDto();

            return JsonSerializer.Deserialize<IntegrationSettingsDto>(user.IntegrationSettingsJson)
                   ?? new IntegrationSettingsDto();
        }

        public async Task UpdateUserIntegrationSettingsAsync(
            Guid userId,
            IntegrationSettingsDto settings)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.IntegrationSettingsJson = JsonSerializer.Serialize(settings);
            await _context.SaveChangesAsync();
        }
    }
}