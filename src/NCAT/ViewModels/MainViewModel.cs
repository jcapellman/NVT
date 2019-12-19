using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.Maps.MapControl.WPF;

using NCAT.lib;
using NCAT.lib.Objects;

namespace NCAT.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event EventHandler<List<NetworkConnectionItem>> OnNewConnections;

        private ObservableCollection<NetworkConnectionItem> _connections;

        public ObservableCollection<NetworkConnectionItem> Connections
        {
            get => _connections;

            set
            {
                _connections = value;

                OnPropertyChanged();
            }
        }

        private ObservableCollection<Location> _locations;

        public ObservableCollection<Location> Locations
        {
            get => _locations;

            set
            {
                _locations = value;

                OnPropertyChanged();
            }
        }

        private BackgroundWorker _bwConnections;

        public MainViewModel()
        {
            Connections = new ObservableCollection<NetworkConnectionItem>();
            Locations = new ObservableCollection<Location>();

            _bwConnections = new BackgroundWorker();

            _bwConnections.DoWork += _bwConnections_DoWork;
            _bwConnections.RunWorkerCompleted += _bwConnections_RunWorkerCompleted;

            _bwConnections.RunWorkerAsync();
        }

        private void _bwConnections_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
        }

        private async void _bwConnections_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var newConnections = await TCPConnections.GetConnectionsAsync();

            var newlyAddedConnections = new List<NetworkConnectionItem>();

            foreach (var connection in newConnections.Where(connection =>
                !Connections.Any(a => a.IPAddress == connection.IPAddress && a.Port == connection.Port)))
            {
                Connections.Insert(0, connection);

                if (connection.ISP == TCPConnections.UNKNOWN)
                {
                    continue;
                }

                Locations.Add(new Location(connection.Latitude, connection.Longitude));

                newlyAddedConnections.Add(connection);
            }

            if (newlyAddedConnections.Any())
            {
                OnNewConnections?.Invoke(null, newlyAddedConnections);
            }

            _bwConnections.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}