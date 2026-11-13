using System.Security.Cryptography;
using System.Text;

namespace Dfe.ManageEntitlementFunding.Web;

public static class DevelopmentExtensions
{
    public static IApplicationBuilder UseDevelopmentAuth(this IApplicationBuilder app)
    {
        var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
        if (!config.GetValue<bool>("DevelopmentBasicAuthEnabled"))
        {
            return app;
        }

        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
        if (!env.IsDevelopment())
        {
            throw new InvalidOperationException("Development Basic Auth can only be enabled in Development.");
        }

        var password = config["DevelopmentBasicAuthPassword"];
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("DevelopmentBasicAuthPassword must be set when Development Basic Auth is enabled.");
        }

        var expectedPasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(DevelopmentExtensions));

        app.Use(async (context, next) =>
        {
            context.Response.Headers.CacheControl = "no-store";

            if (!HasValidCredentials(context.Request.Headers.Authorization.ToString(), expectedPasswordHash))
            {
                logger.LogWarning(
                    "Development Basic Auth rejected request. Method: {Method}, Path: {Path}",
                    context.Request.Method,
                    context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers.WWWAuthenticate = "Basic realm=\"Development\", charset=\"UTF-8\"";
                return;
            }

            await next();
        });

        return app;
    }

    private static bool HasValidCredentials(string authorizationHeader, byte[] expectedPasswordHash)
    {
        const string prefix = "Basic ";
        if (!authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string credentials;
        try
        {
            credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationHeader[prefix.Length..]));
        }
        catch (FormatException)
        {
            return false;
        }

        var separatorIndex = credentials.IndexOf(':');
        if (separatorIndex < 0)
        {
            return false;
        }

        // As in Accessing Childcare, the username is arbitrary; only the shared password is checked.
        var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(credentials[(separatorIndex + 1)..]));
        return CryptographicOperations.FixedTimeEquals(passwordHash, expectedPasswordHash);
    }
}
