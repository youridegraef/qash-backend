namespace Application.Dtos;

public class AuthenticationDto(string token, User user)
{
    public string Token { get; set; } = token;
    public User User { get; set; } = user;
    //User velden los invullen hier
}