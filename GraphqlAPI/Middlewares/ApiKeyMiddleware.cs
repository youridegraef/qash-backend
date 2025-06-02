using System.Security.Claims;

namespace GraphqlAPI.Middlewares;

public class ApiKeyMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    ILogger<ApiKeyMiddleware> logger) {
    private const string ApikeyHeader = "X-API-KEY";

    public async Task InvokeAsync(HttpContext context) {
        // Probeer eerst of er al een gebruiker geauthenticeerd is (bijv. via JWT)
        // Als je wilt dat API key authenticatie JWT overschrijft of als fallback dient,
        // kun je de logica hier aanpassen.
        // Voor nu gaan we ervan uit dat als er een API key is, we die proberen te valideren.

        if (context.Request.Headers.TryGetValue(ApikeyHeader, out var extractedApiKey)) {
            var serverApiKey = configuration["ApiKey"];

            if (string.IsNullOrEmpty(serverApiKey)) {
                logger.LogError("Server API Key is not configured.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("API Key configuration error.");
                return;
            }

            if (serverApiKey.Equals(extractedApiKey.ToString())) // ToString() voor de zekerheid
            {
                logger.LogInformation(
                    "Valid API Key received. Authenticating request via API Key."
                );
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, "ApiKeyUser"),
                    new Claim("AuthMethod", "ApiKey")
                    // Je hoeft hier geen specifieke rollen toe te voegen als de key alles mag
                    // en je geen rol-gebaseerde [Authorize] attributen gebruikt.
                };
                var identity = new ClaimsIdentity(claims, "ApiKeyAuthentication");
                context.User = new ClaimsPrincipal(identity); // << DIT IS DE CRUCIALE TOEVOEGING
            }
            else {
                logger.LogWarning(
                    "Invalid API Key received: {ProvidedKey}",
                    extractedApiKey
                );
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
        }
        // Als de header niet aanwezig is, en je wilt dat dit een harde eis is:
        // else if (/* een conditie waaronder API key verplicht is */)
        // {
        //     _logger.LogWarning("API Key header is missing.");
        //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //     await context.Response.WriteAsync("API Key is missing");
        //     return;
        // }
        // Als de API key optioneel is en JWT ook gebruikt kan worden,
        // dan wil je hier niet per se een 401 teruggeven als de header mist.
        // De code in je oorspronkelijke middleware doet dit wel, wat prima is als de API key altijd verwacht wordt.

        await next(context);
    }
}