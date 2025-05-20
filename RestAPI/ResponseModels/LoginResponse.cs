namespace RestAPI.ResponseModels;

public class LoginResponse(string token, User user)
{
    public string Token { get; set; } = token;
    public User User { get; set; } = user;
}