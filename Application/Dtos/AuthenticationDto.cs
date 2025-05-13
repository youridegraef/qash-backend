namespace Application.Dtos;

public class AuthenticationDto
{
    public string Token { get; set; }
    public User User { get; set; }

    public AuthenticationDto(string token, User user)
    {
        Token = token;
        User = user;
    }
}