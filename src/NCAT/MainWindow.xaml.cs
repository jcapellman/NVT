using System;
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

            if (!ViewModel.Locations.Any())
            {
                return;
            }

            foreach (var item in ViewModel.Connections.Where(a => a.Latitude.HasValue && a.Longitude.HasValue))
            {
                bmMap.Children.Add(new Pushpin
                {
                    Location = new Location(item.Latitude.Value, item.Longitude.Value),
                    ToolTip = item.ISP
                });
            }

            if (ViewModel.Locations.Count == 1)
            {
                bmMap.Center = ViewModel.Locations.FirstOrDefault();

                return;
            }

            try
            {
                var boundingBox = new LocationRect(ViewModel.Locations.OrderBy(a => a.Latitude).ToList());

                bmMap.SetView(boundingBox);
                bmMap.ZoomLevel *= 0.85;
            } catch (Exception ex)
            {
                // LOG
            }
        }

        private void btnSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foSettings.IsOpen = !foSettings.IsOpen;
        }

        private void btnAbout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foAbout.IsOpen = !foAbout.IsOpen;
        }
    }
}