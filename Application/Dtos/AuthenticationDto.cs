namespace Application.Dtos;

public class AuthenticationDto
{
    public string Token { get; set; }
    public UserAuthenticate UserAuthenticate { get; set; }

    public AuthenticationDto(string token, UserAuthenticate userAuthenticate)
    {
        Token = token;
        UserAuthenticate = userAuthenticate;
    }
}