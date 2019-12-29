using System.Windows;

using NVT.lib;
using NVT.lib.Windows.PlatformImplementations;

namespace NVT
{
    public partial class App : Application
    {
        public App()
        {
            DIContainer.BuildContainer(new WindowsNetworkConnectionQuery());
        }
    }
}