using Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BLL.DTOs.Workout;

public class WorkoutCreateDto
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WorkoutType Type { get; set; }

    [Required, Range(1, 1000)]
    public int Duration { get; set; }

    [Range(0, 10000)]
    public int Calories { get; set; }

    [Range(0, 1000)]
    public double? Distance { get; set; }

    public int? AverageHeartRate { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public string Notes { get; set; }
}