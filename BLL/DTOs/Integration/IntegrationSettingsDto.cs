namespace BLL.DTOs.Integration;

public class IntegrationSettingsDto
{
    public bool GoogleFitEnabled { get; set; }
    public bool AppleHealthEnabled { get; set; }
    public int SyncFrequencyHours { get; set; } = 24;
    public DateTime? LastSyncDate { get; set; }
}