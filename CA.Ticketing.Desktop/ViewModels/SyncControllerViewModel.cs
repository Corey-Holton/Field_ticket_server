using CA.Ticketing.Desktop.Base;
using CA.Ticketing.Desktop.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CA.Ticketing.Desktop.ViewModels
{
    public class SyncControllerViewModel : ViewModelBase
    {
        private string _serverStatus = "-";

        private bool _manualSyncInProgress = false;

        private string _statusLastChecked = "-";

        private bool _canRunSync = false;

        private readonly Action _reloadAction;

        private readonly ServerConnector _serverConnector;

        private System.Timers.Timer _serviceStatusTimer;

        private readonly double _serviceStatusInterval = TimeSpan.FromSeconds(5).TotalMilliseconds;

        private bool _canReload;

        public string ServerStatus
        {
            get { return _serverStatus; }
            set
            {
                _serverStatus = value;
                OnPropertyChange(nameof(ServerStatus));
            }
        }

        public string StatusLastChecked
        {
            get { return _statusLastChecked; }
            set
            {
                _statusLastChecked = value;
                OnPropertyChange(nameof(StatusLastChecked));
            }
        }

        public bool CanRunSync
        {
            get { return _canRunSync; }
            set
            {
                _canRunSync = value;
                OnPropertyChange(nameof(CanRunSync));
            }
        }

        public bool CanReload
        {
            get { return _canReload; }
            set
            {
                _canReload = value;
                OnPropertyChange(nameof(CanReload));
            }
        }

        public ICommand ReloadCommand => new RunCommand((o) => _reloadAction?.Invoke());

        public ICommand RunSyncCommand => new RunCommand(RunSync);

        public event EventHandler ServerInitialized;

        public event EventHandler<bool> ShowSyncProgress;

        public event EventHandler<string> ShowServerMessage;

        public SyncControllerViewModel(ServerConnector serverConnector, Action reloadAction)
        {
            _serverConnector = serverConnector;
            _reloadAction = reloadAction;
        }

        public async Task Init()
        {
            var isServerAvailable = await _serverConnector.IsServerAvailable();
            
            if (!isServerAvailable)
            {
                throw new Exception("Unable to reach server. Please check the web application and restart the application.");
            }

            _serviceStatusTimer = new System.Timers.Timer(_serviceStatusInterval)
            {
                AutoReset = false
            };

            _serviceStatusTimer.Elapsed += CheckServiceStatus;
            CheckServiceStatus(null, null);
        }

        public void Dispose()
        {
            _serviceStatusTimer?.Dispose();
        }

        private void CheckServiceStatus(object? sender, System.Timers.ElapsedEventArgs? e)
        {
            try
            {
                CheckService().Wait();
            }
            catch
            {
            }
            finally
            {
                _serviceStatusTimer.Start();
            }
        }

        private async Task CheckService()
        {
            if (_manualSyncInProgress)
            {
                return;
            }

            CanRunSync = false;

            var serverStatus = await _serverConnector.GetServerStatus();

            if (serverStatus == null)
            {
                throw new Exception("Unable to get server status");
            }

            ServerStatus = serverStatus.IsOnline ? "Online" : "Offline";
            StatusLastChecked = serverStatus.LastSyncDate.HasValue ? serverStatus.LastSyncDate.Value.ToLocalTime().ToString() : "Never";
            
            if (serverStatus.InitialSyncInProgress)
            {
                StatusLastChecked += $" - Initial Sync in progress";
                CanReload = false;
            }
            else
            {
                StatusLastChecked += $" - Next sync in {serverStatus.NextSyncIn} minutes";
                CanReload = true;
                ServerInitialized?.Invoke(this, null);
            }

            CanRunSync = !serverStatus.SyncInProgress && serverStatus.IsOnline;
        }

        public async void RunSync(object o)
        {
            CanRunSync = false;
            _manualSyncInProgress = true;
            ShowSyncProgress?.Invoke(this, true);
            try
            {
                await _serverConnector.RunSync();
                _manualSyncInProgress = false;
                await CheckService();
                CanRunSync = true;
                ShowSyncProgress?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                ShowServerMessage?.Invoke(this, ex.Message);
            }
            finally
            {
                _manualSyncInProgress = false;
            }
        }
    }
}
