namespace Presentation.Middleware;

public class UseAuth
{
    private readonly RequestDelegate _next;

    public UseAuth(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        //TODO: maak een middleware die checkt of gebruker is ingelogd ofniet
        
        //Als ingelogd: Redirect > /index
        //Als niet ingelogd: Redirect > /login
    }
}