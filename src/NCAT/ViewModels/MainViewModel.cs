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
        public event EventHandler OnNewConnections;

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

        public List<Location> Locations =>
            Connections.Where(a => a.Latitude.HasValue && a.Longitude.HasValue).Select(a => new Location(a.Latitude.Value, a.Longitude.Value)).ToList();

        private BackgroundWorker _bwConnections;

        public MainViewModel()
        {
            Connections = new ObservableCollection<NetworkConnectionItem>();
            
            _bwConnections = new BackgroundWorker();

            _bwConnections.DoWork += _bwConnections_DoWork;
            _bwConnections.RunWorkerCompleted += _bwConnections_RunWorkerCompleted;

            _bwConnections.RunWorkerAsync();
        }

        private void _bwConnections_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(3000);
        }

        private async void _bwConnections_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var newConnections = await TCPConnections.GetConnectionsAsync();

            for (var x = 0; x < Connections.Count; x++)
            {
                if (!newConnections.Any(a => a.IPAddress == Connections[x].IPAddress && a.Port == Connections[x].Port))
                {
                    Connections.RemoveAt(x);
                }
            }

            foreach (var connection in newConnections.Where(connection =>
                !Connections.Any(a => a.IPAddress == connection.IPAddress && a.Port == connection.Port)))
            {
                Connections.Insert(0, connection);
            }

            OnNewConnections?.Invoke(null, null);

            _bwConnections.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}