using Common.Enums;

namespace BLL.Interfaces;

public interface IIntegrationService
{
    Task ImportWorkoutsFromExternalServiceAsync(Guid userId, IntegrationSourceType serviceType);
    Task SyncWithExternalServiceAsync(Guid userId, IntegrationSourceType serviceType);
}
