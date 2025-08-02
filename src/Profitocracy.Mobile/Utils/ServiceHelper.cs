namespace Profitocracy.Mobile.Utils;

public static class ServiceHelper
{
    private static IServiceProvider? Services { get; set; }

    public static void Initialize(IServiceProvider services) => Services = services;

    public static T GetService<T>() =>
        (Services != null
            ? Services.GetService<T>()
            : throw new InvalidOperationException("IServiceProvider is not available.")) ??
        throw new InvalidOperationException($"{typeof(T)} is not available.");
}
