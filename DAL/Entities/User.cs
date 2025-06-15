using Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     
    public RefreshToken RefreshToken { get; set; }

    public ICollection<Workout> Workouts { get; set; }
    public ICollection<Goal> Goals { get; set; }
}
