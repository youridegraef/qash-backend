namespace Application.Dtos;

public class AuthenticationDto(int id, string name, string email, string token) {
    public int Id { get; private set; }
    public string Token { get; private set; } = token;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
}