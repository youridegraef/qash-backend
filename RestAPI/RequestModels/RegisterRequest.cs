namespace RestAPI.RequestModels;

public class RegisterRequest(int id, string name, string email, string password) {
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}