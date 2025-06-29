using BLL.DTOs.Integration;
using Common.Enums;

namespace BLL.Interfaces
{
    public interface IIntegrationService
    {
        Task ImportWorkoutsFromExternalServiceAsync(
            Guid userId,
            IntegrationSourceType serviceType,
            CancellationToken cancellationToken = default);

        Task SyncWithExternalServiceAsync(
            Guid userId,
            IntegrationSourceType serviceType,
            CancellationToken cancellationToken = default);

        Task<IntegrationSettingsDto> GetUserIntegrationSettingsAsync(Guid userId);
        Task UpdateUserIntegrationSettingsAsync(Guid userId, IntegrationSettingsDto settings);
    }
}