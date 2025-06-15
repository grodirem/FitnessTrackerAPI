using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.User;

public class UserUpdateDto
{
    [MaxLength(100)]
    public string Name { get; set; }

    public DateTime BirthDate { get; set; }

    public Gender? Gender { get; set; }
}