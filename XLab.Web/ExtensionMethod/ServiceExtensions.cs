using System.Reflection;

namespace XLab.Web.ExtensionMethod;

public static class ServiceExtensions
{
    public static void RegisterAllServices(this IServiceCollection services, Type type)
    {
        var installer = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type.IsAssignableFrom)
            .Where(x => !x.IsAbstract && !x.IsGenericType && x.IsClass)
            .Select(x => new
            {
                Interface = x.GetInterfaces().First(),
                Implementation = x
            })
            .ToList();

        installer.ForEach(x => { services.AddScoped(x.Interface, x.Implementation); });
    }
}