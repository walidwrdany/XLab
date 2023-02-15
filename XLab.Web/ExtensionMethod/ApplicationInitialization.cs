using XLab.Common.Interfaces;

namespace XLab.Web.ExtensionMethod;

public class ApplicationInitLogger { }

public static class ApplicationInitialization
{
    /// <summary>
    /// Initializes objects on initialization of the Application.
    /// For example - Initialize all cache stores
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static IHost AppInitialize(this IHost host)
    {
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        var logger = host.Services.GetRequiredService<ILogger<ApplicationInitLogger>>();

        // We need to create a scope as host only has the root scope, and our services are scoped.
        using var scope = scopeFactory.CreateScope();

        // Any class that has some data to be initialized must implement ISupportApplicationInitialization
        var interfaceType = typeof(ISupportApplicationInitialization);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => interfaceType.IsAssignableFrom(p));

        foreach (var type in types)
        {
            try
            {
                var implementingClass = scope.ServiceProvider.GetService(type) as ISupportApplicationInitialization;
                // Some classes are registered as their interfaces only, hence we do a null check.

                logger.LogInformation($"Executing app init for {type.Name}. Class is null:{implementingClass == null}");

                implementingClass?.OnAppInit();
            }
            catch (Exception ex)
            {
                // Do Nothing
                logger.LogError(ex, "Error in application initialization");
            }
        }

        return host;
    }
}