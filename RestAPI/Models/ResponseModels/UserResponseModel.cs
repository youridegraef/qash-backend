namespace RestAPI.Models.ResponseModels;

public class UserResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public UserResponseModel(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}