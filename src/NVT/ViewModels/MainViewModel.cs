using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;

using Microsoft.Maps.MapControl.WPF;

using NVT.lib.JSONObjects;
using NVT.lib.Managers;
using NVT.lib.Objects;

using NLog;
using NVT.lib;
using LogConfigurationManager = NVT.lib.Managers.LogConfigurationManager;

namespace NVT.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private bool _exportBtnEnabled;

        public bool ExportBtnEnabled
        {
            get => _exportBtnEnabled;

            set
            {
                _exportBtnEnabled = value; 
                
                OnPropertyChanged();
            }
        }

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

        private string _currentStatus;

        public string CurrentStatus
        {
            get => _currentStatus;

            set
            {
                _currentStatus = value;
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

                ExportBtnEnabled = Connections.Any();
            }
        }

        public List<Location> Locations =>
            Connections.Where(a => a.Latitude.HasValue && a.Longitude.HasValue).Select(a => new Location(a.Latitude.Value, a.Longitude.Value)).ToList();

        public string SaveSettings()
        {
            var result = DIContainer.GetDIService<SettingsManager>().WriteFile();

            LogConfigurationManager.AdjustLogLevel(SettingsObject.LogLevel);

            return result ? lib.Resources.AppResources.MainViewModel_Settings_SavedSuccessfully : lib.Resources.AppResources.MainViewModel_Settings_SavedUnsuccessfully;
        }

        private BackgroundWorker _bwConnections;

        public string ExportConnections(string fileName)
        {
            if (Connections == null || !Connections.Any())
            {
                return NVT.lib.Resources.AppResources.MainWindowCommand_Export_Message_NoConnections;
            }

            try
            {
                var json = JsonSerializer.Serialize(Connections);

                File.WriteAllText(fileName, json);

                return $"{NVT.lib.Resources.AppResources.MainWindowCommand_Export_Message_Success} {fileName}";
            } catch (Exception ex)
            {
                Log.Error($"Exception occurred when exporting to {fileName}: {ex}");

                return NVT.lib.Resources.AppResources.MainWindowCommand_Export_Message_Error;
            }
        }

        public SettingsObject SettingsObject
        {
            get => DIContainer.GetDIService<SettingsManager>().SettingsObject;

            set
            {
                DIContainer.GetDIService<SettingsManager>().SettingsObject = value;

                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            MainGridVisibility = Visibility.Collapsed;
            EmptyGridVisibility = Visibility.Visible;

            ExportBtnEnabled = false;

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
            var newConnections = await DIContainer.GetDIService<ConnectionManager>().GetConnectionsAsync();

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

            CurrentStatus = Connections.Count == 1 ? 
                $"{Connections.Count} {NVT.lib.Resources.AppResources.MainViewModel_ConnectionStatus_Singular}" : 
                $"{Connections.Count} {NVT.lib.Resources.AppResources.MainViewModel_ConnectionStatus_Plural}";

            _bwConnections.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string StopProcess(NetworkConnectionItem networkConnectionItem) => ProcessManager.KillProcess(networkConnectionItem.ProcessId);
    }
}