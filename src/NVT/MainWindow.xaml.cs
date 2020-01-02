using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;

using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;

using NVT.Extensions;
using NVT.ViewModels;
using NVT.lib.Objects;

using NLog;

namespace NVT
{
    public partial class MainWindow : MetroWindow
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private MainViewModel ViewModel => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();

            
            Closing += MainWindow_Closing;

            DataContext = new MainViewModel();

            ViewModel.OnNewConnections += ViewModel_OnNewConnections;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            Hide();
        }

        private void ViewModel_OnNewConnections(object sender, System.EventArgs e)
        {
            if (!ViewModel.SettingsObject.EnableMap)
            {
                return;
            }

            bmMap.Children.Clear();

            if (!ViewModel.Locations.Any())
            {
                Log.Debug("No locations found - not updating the map");

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
                Log.Debug("Only 1 location found, setting the Center of the Map to the location");

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
                Log.Error($"Exception when setting the view: {ex}");
            }
        }

        private void btnSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foSettings.IsOpen = !foSettings.IsOpen;

            if (foSettings.IsOpen)
            {
                foSettings.BringToFront();
            }
        }

        private void btnAbout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foAbout.IsOpen = !foAbout.IsOpen;

            if (foAbout.IsOpen)
            {
                foAbout.BringToFront();
            }
        }

        private void btnExport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var ofd = new SaveFileDialog
            {
                DefaultExt = ".json",

                Filter = "json|*.json",

                AddExtension = true
            };

            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            var response = ViewModel.ExportConnections(ofd.FileName);

            MessageBox.Show(response);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var (message, success) = ViewModel.SaveSettings();

            MessageBox.Show(message);

            if (success)
            {
                btnSettings_Click(null, null);
            }
        }

        private void btnStopProcess_Click(object sender, RoutedEventArgs e)
        {
            var response = ViewModel.StopProcess((NetworkConnectionItem)((Button) sender).DataContext);

            MessageBox.Show(response);
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(((NetworkConnectionItem)((Button)sender).DataContext).IPAddress);

            MessageBox.Show(lib.Resources.AppResources.MainWindow_Copy_IPAddress);
        }

        private void btnViewLog_Click(object sender, RoutedEventArgs e)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, lib.Common.Constants.LOG_FILENAME);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show($"{lib.Resources.AppResources.MainWindow_LogFileNotFound} ({lib.Common.Constants.LOG_FILENAME})");

                return;
            }

            Process.Start(fullPath);
        }

        private void hlWebpage_Click(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }

        private void mnuItem_ShowWindow(object sender, RoutedEventArgs e)
        {
            if (Visibility != Visibility.Visible)
            {
                Visibility = Visibility.Visible;
            }
        }

        private void mnuItem_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}