namespace RestAPI.RequestModels;

public class UserRequest(int id, string name, string email, string password) {
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}