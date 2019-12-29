using Microsoft.Extensions.DependencyInjection;

using NVT.lib.Managers;
using NVT.lib.PlatformAbstractions;

namespace NVT.lib
{
    public static class DIContainer
    {
        private static ServiceProvider _container;

        public static T GetDIService<T>() => _container.GetService<T>();

        public static void BuildContainer(BaseNetworkConnectionQuery connectionQuery)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(new ConnectionManager(connectionQuery));

            _container = serviceCollection.BuildServiceProvider();

            serviceCollection.AddSingleton(new SettingsManager());

            _container = serviceCollection.BuildServiceProvider();
        }
    }
}