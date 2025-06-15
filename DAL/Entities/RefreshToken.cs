using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public DateTime Expires { get; set; }

    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => !IsExpired;

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}
