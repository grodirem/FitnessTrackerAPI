using DAL.Interfaces;
using DAL.Repositories;
using FluentValidation;

namespace API.Extensions;

public static class ServicesRegistrationExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        //
    }

    public static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
    }
}
