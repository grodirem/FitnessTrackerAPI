using System.ComponentModel.DataAnnotations;

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
    public Guid UserId { get; set; }

    public User User { get; set; }
}
