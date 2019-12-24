using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

using Microsoft.Maps.MapControl.WPF;

using NCAT.lib.Managers;
using NCAT.lib.Objects;

using NLog;

namespace NCAT.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private Visibility _mainGridVisibility;

        public Visibility MainGridVisibility
        {
            get => _mainGridVisibility;

            set
            {
                _mainGridVisibility = value;

                OnPropertyChanged();
            }
        }

        private Visibility _emptyGridVisibility;

        public Visibility EmptyGridVisibility
        {
            get => _emptyGridVisibility;

            set
            {
                _emptyGridVisibility = value;

                OnPropertyChanged();
            }
        }

        private Visibility _mapVisibility;

        public Visibility MapVisibility
        {
            get => _mapVisibility;

            set
            {
                _mapVisibility = value;

                OnPropertyChanged();
            }
        }

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

        private string _currenStatus;

        public string CurrentStatus
        {
            get => _currenStatus;

            set
            {
                _currenStatus = value;
                OnPropertyChanged();

                if (Connections.Any())
                {
                    EmptyGridVisibility = Visibility.Collapsed;
                    MainGridVisibility = Visibility.Visible;
                } else
                {
                    EmptyGridVisibility = Visibility.Visible;
                    MainGridVisibility = Visibility.Collapsed;
                }

                MapVisibility = Locations.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public List<Location> Locations =>
            Connections.Where(a => a.Latitude.HasValue && a.Longitude.HasValue).Select(a => new Location(a.Latitude.Value, a.Longitude.Value)).ToList();

        private BackgroundWorker _bwConnections;

        private readonly ConnectionManager connectionManager = new ConnectionManager();

        public MainViewModel()
        {
            MainGridVisibility = Visibility.Collapsed;
            EmptyGridVisibility = Visibility.Visible;

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
            var newConnections = await connectionManager.GetConnectionsAsync(connectionManager.SupportedConnectionTypes);

            Log.Debug($"Received {newConnections.Count} connections");

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

            CurrentStatus = $"{Connections.Count} connection(s) found";

            _bwConnections.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}