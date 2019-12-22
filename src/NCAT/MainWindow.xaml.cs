using System.Linq;

using MahApps.Metro.Controls;

using Microsoft.Maps.MapControl.WPF;

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

            ViewModel.OnNewConnections += ViewModel_OnNewConnections;
        }

        private void ViewModel_OnNewConnections(object sender, System.EventArgs e)
        {
            bmMap.Children.Clear();

            foreach (var item in ViewModel.Connections.Where(a => a.Latitude.HasValue && a.Longitude.HasValue))
            {
                bmMap.Children.Add(new Pushpin
                {
                    Location = new Location(item.Latitude.Value, item.Longitude.Value),
                    ToolTip = item.ISP
                });
            }

            var boundingBox = new LocationRect(ViewModel.Locations);
            
            bmMap.SetView(boundingBox);
            bmMap.ZoomLevel *= 0.85;
        }
    }
}