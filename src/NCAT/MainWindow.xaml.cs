using System.Linq;
using MahApps.Metro.Controls;

using Microsoft.Maps.MapControl.WPF;

using NCAT.ViewModels;

namespace NCAT
{
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel viewModel => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            viewModel.OnNewConnections += ViewModel_OnNewConnections;    
        }

        private void ViewModel_OnNewConnections(object sender, System.Collections.Generic.List<lib.Objects.NetworkConnectionItem> e)
        {
            foreach (var item in e)
            {
                bmMap.Children.Add(new Pushpin
                {
                    Location = new Location(item.Latitude, item.Longitude),
                    ToolTip = item.ISP
                });
            }

            LocationRect boundingBox = new LocationRect(viewModel.Locations);

            bmMap.SetView(boundingBox);
        }
    }
}