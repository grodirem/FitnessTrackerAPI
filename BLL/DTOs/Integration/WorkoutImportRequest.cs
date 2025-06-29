using Common.Enums;

namespace BLL.DTOs.Integration;

public class WorkoutImportRequest
{
    public Guid UserId { get; set; }
    public IntegrationSourceType ServiceType { get; set; }
}
