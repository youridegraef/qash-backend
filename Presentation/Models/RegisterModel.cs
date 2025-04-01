using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Presentation.Models;

public class RegisterModel
{
    [Required] [EmailAddress] public string email { get; set; }
    public string name { get; set; }
    [Required] public string password { get; set; }
    public DateOnly dateOfBirth { get; set; }
}