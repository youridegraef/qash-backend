namespace RestAPI.Middlewares;

public class ApiKeyMiddleware(RequestDelegate next) {
    private const string ApikeyHeader = "X-API-KEY";

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration) {
        if (!context.Request.Headers.TryGetValue(ApikeyHeader, out var extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        var apiKey = configuration.GetValue<string>("ApiKey")!;

        if (!apiKey.Equals(extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        await next(context);
    }
}