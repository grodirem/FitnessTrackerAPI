using Common.Enums;

namespace BLL.DTOs.Statistics;

public class StatisticsRequestDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PeriodType PeriodType { get; set; } = PeriodType.Month;
}
