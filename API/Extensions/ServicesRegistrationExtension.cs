using BLL.Interfaces;
using BLL.Services;
using BLL.Validators.Integration;
using BLL.Validators.Statistics;
using BLL.Validators.User;
using BLL.Validators.Workout;
using DAL.Interfaces;
using DAL.Repositories;
using FluentValidation;

namespace API.Extensions;

public static class ServicesRegistrationExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IGoalRepository, GoalRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGoalService, GoalService>();
        services.AddScoped<IWorkoutService, WorkoutService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IIntegrationService, IntegrationService>();
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserLoginDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UserUpdateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<WorkoutCreateDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<StatisticsRequestDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<ExternalWorkoutValidator>();
        services.AddValidatorsFromAssemblyContaining<NotificationSettingsUpdateValidator>();
    }
}
