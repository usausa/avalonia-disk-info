namespace DiskInfo;

using DiskInfo.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Smart.Avalonia;
using Smart.Resolver;

public static class ApplicationExtensions
{
    //--------------------------------------------------------------------------------
    // Components
    //--------------------------------------------------------------------------------

    public static HostApplicationBuilder ConfigureComponents(this HostApplicationBuilder builder)
    {
        builder.Services.AddAvaloniaServices();
        builder.Services.AddSingleton<IDiskInfoProvider>(static serviceProvider => CreateDiskInfoProvider(serviceProvider));
        builder.Services.AddSingleton<MainWindowViewModel>();

        builder.ConfigureContainer(new SmartServiceProviderFactory(), ConfigureContainer);

        return builder;
    }

    private static void ConfigureContainer(ResolverConfig config)
    {
        config
            .UseAutoBinding()
            .UseArrayBinding()
            .UseAssignableBinding();

        // Messenger
        config.BindSingleton<IReactiveMessenger>(ReactiveMessenger.Default);

        // Window
        config.BindSingleton<MainWindow>();
    }

    private static IDiskInfoProvider CreateDiskInfoProvider(IServiceProvider serviceProvider)
    {
        if (OperatingSystem.IsWindows())
        {
            return ActivatorUtilities.CreateInstance<WindowsDiskInfoProvider>(serviceProvider);
        }

        if (OperatingSystem.IsLinux())
        {
            return ActivatorUtilities.CreateInstance<LinuxDiskInfoProvider>(serviceProvider);
        }

        if (OperatingSystem.IsMacOS())
        {
            return ActivatorUtilities.CreateInstance<MacDiskInfoProvider>(serviceProvider);
        }

        throw new PlatformNotSupportedException();
    }

    //--------------------------------------------------------------------------------
    // Startup
    //--------------------------------------------------------------------------------

    public static async ValueTask StartApplicationAsync(this IHost host)
    {
        await host.StartAsync().ConfigureAwait(false);
    }

    public static async ValueTask ExitApplicationAsync(this IHost host)
    {
        // Stop host
        await host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        host.Dispose();
    }
}
