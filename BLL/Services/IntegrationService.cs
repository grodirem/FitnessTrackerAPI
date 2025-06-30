using AutoMapper;
using BLL.DTOs.Integration;
using BLL.Interfaces;
using Common.Enums;
using DAL.Contexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BLL.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly FitnessTrackerContext _context;
        private readonly IMapper _mapper;
        private readonly IWorkoutService _workoutService;
        private readonly IValidator<ExternalWorkoutDto> _validator;
        private readonly ILogger<IntegrationService> _logger;
        private readonly Random _random = new();

        public IntegrationService(
            FitnessTrackerContext context,
            IMapper mapper,
            IWorkoutService workoutService,
            IValidator<ExternalWorkoutDto> validator,
            ILogger<IntegrationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _workoutService = workoutService;
            _validator = validator;
            _logger = logger;
        }

        public async Task ImportWorkoutsFromExternalServiceAsync(
            Guid userId,
            IntegrationSourceType serviceType,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting import from {ServiceType} for user {UserId}", serviceType, userId);

            var mockWorkouts = GenerateMockWorkouts(serviceType);
            int importedCount = 0;

            foreach (var workoutDto in mockWorkouts)
            {
                try
                {
                    await _validator.ValidateAndThrowAsync(workoutDto, cancellationToken);

                    if (!await CheckIfWorkoutExistsAsync(userId, workoutDto, cancellationToken))
                    {
                        await _workoutService.CreateWorkoutFromIntegrationAsync(
                            userId, workoutDto, cancellationToken);
                        importedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to import workout: {Message}", ex.Message);
                }
            }

            _logger.LogInformation("Imported {Count} new workouts from {ServiceType}", importedCount, serviceType);
        }

        public async Task SyncWithExternalServiceAsync(
            Guid userId,
            IntegrationSourceType serviceType,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting sync with {ServiceType} for user {UserId}", serviceType, userId);

            var settings = await GetUserIntegrationSettingsAsync(userId);

            if ((serviceType == IntegrationSourceType.GoogleFit && !settings.GoogleFitEnabled) ||
                (serviceType == IntegrationSourceType.AppleHealth && !settings.AppleHealthEnabled))
            {
                throw new InvalidOperationException($"Integration with {serviceType} is disabled");
            }

            var lastSyncDate = settings.LastSyncDate ?? DateTime.UtcNow.AddDays(-7);
            _logger.LogDebug("Last sync date: {LastSyncDate}", lastSyncDate);

            var newWorkouts = GenerateMockWorkouts(serviceType)
                .Where(w => w.StartTime > lastSyncDate)
                .ToList();

            int addedCount = 0;
            foreach (var workoutDto in newWorkouts)
            {
                try
                {
                    if (!await CheckIfWorkoutExistsAsync(userId, workoutDto, cancellationToken))
                    {
                        await _workoutService.CreateWorkoutFromIntegrationAsync(
                            userId, workoutDto, cancellationToken);
                        addedCount++;
                        _logger.LogDebug("Added new workout: {Type} at {StartTime}",
                            workoutDto.Type, workoutDto.StartTime);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync workout: {Message}", ex.Message);
                }
            }

            settings.LastSyncDate = DateTime.UtcNow;
            await UpdateUserIntegrationSettingsAsync(userId, settings);

            _logger.LogInformation("Sync completed. Added {Count} new workouts", addedCount);
        }

        private async Task<bool> CheckIfWorkoutExistsAsync(
            Guid userId,
            ExternalWorkoutDto workoutDto,
            CancellationToken cancellationToken)
        {
            var date = workoutDto.StartTime.Date;
            var minDate = date.AddDays(-1);
            var maxDate = date.AddDays(1);
            var minDuration = workoutDto.Duration - 10;
            var maxDuration = workoutDto.Duration + 10;

            return await _context.Workouts
                .AnyAsync(w => w.UserId == userId
                    && w.Type == workoutDto.Type
                    && w.Date >= minDate
                    && w.Date <= maxDate
                    && w.Duration >= minDuration
                    && w.Duration <= maxDuration,
                cancellationToken);
        }

        private List<ExternalWorkoutDto> GenerateMockWorkouts(IntegrationSourceType serviceType)
        {
            var mockWorkouts = new List<ExternalWorkoutDto>();
            var workoutTypes = Enum.GetValues<WorkoutType>();
            var daysToGenerate = 30;

            for (int i = 0; i < 100; i++)
            {
                var workoutType = workoutTypes[_random.Next(workoutTypes.Length)];
                var daysAgo = _random.Next(0, daysToGenerate);
                var startTime = DateTime.UtcNow.AddDays(-daysAgo);
                var endTime = startTime.AddMinutes(_random.Next(15, 120));

                mockWorkouts.Add(new ExternalWorkoutDto
                {
                    Type = workoutType,
                    Duration = (int)(endTime - startTime).TotalMinutes,
                    Calories = _random.Next(100, 800),
                    Distance = workoutType is WorkoutType.Running or WorkoutType.Cycling
                        ? _random.Next(1, 20)
                        : null,
                    StartTime = startTime,
                    EndTime = endTime,
                    Source = serviceType,
                    Notes = $"Mock {Guid.NewGuid():N} from {serviceType} at {startTime:yyyy-MM-dd HH:mm}"
                });
            }

            return mockWorkouts;
        }

        public async Task<IntegrationSettingsDto> GetUserIntegrationSettingsAsync(Guid userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrEmpty(user?.IntegrationSettingsJson))
                return new IntegrationSettingsDto();

            try
            {
                return JsonSerializer.Deserialize<IntegrationSettingsDto>(user.IntegrationSettingsJson)
                       ?? new IntegrationSettingsDto();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize settings for user {UserId}", userId);
                return new IntegrationSettingsDto();
            }
        }

        public async Task UpdateUserIntegrationSettingsAsync(
            Guid userId,
            IntegrationSettingsDto settings)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException($"User {userId} not found");

            user.IntegrationSettingsJson = JsonSerializer.Serialize(settings);
            await _context.SaveChangesAsync();
        }
    }
}