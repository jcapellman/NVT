using System.Linq;

using MahApps.Metro.Controls;

using Microsoft.Maps.MapControl.WPF;

using NCAT.lib;
using NCAT.ViewModels;

namespace NCAT
{
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel ViewModel => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            ViewModel.OnNewConnections += ViewModel_OnNewConnections1; ;    
        }

        private void ViewModel_OnNewConnections1(object sender, System.EventArgs e)
        {
            bmMap.Children.Clear();

            foreach (var item in ViewModel.Connections.Where(a => a.ISP != TCPConnections.UNKNOWN))
            {
                bmMap.Children.Add(new Pushpin
                {
                    Location = new Location(item.Latitude, item.Longitude),
                    ToolTip = item.ISP
                });
            }

            var boundingBox = new LocationRect(ViewModel.Locations);
            
            bmMap.SetView(boundingBox);
            bmMap.ZoomLevel = 3;
        }
    }
}