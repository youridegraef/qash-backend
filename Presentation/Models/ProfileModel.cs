namespace Presentation.Models;

public class ProfileModel
{
    public string email { get; set; }
    public string name { get; set; }
    public string password { get; set; }
    public DateOnly dateOfBirth { get; set; }
}