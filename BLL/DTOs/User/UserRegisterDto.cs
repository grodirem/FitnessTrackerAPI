using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.User;

public class UserRegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public Gender Gender { get; set; }
}