using CA.Ticketing.Common.Constants;
using CA.Ticketing.Desktop.Base;
using CA.Ticketing.Desktop.Controls;
using System;
using System.Threading.Tasks;

namespace CA.Ticketing.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Services.ServerConnector _serverConnector;

        private readonly SyncControllerViewModel _syncControllerModel;

        private bool _webViewVisible = false;

        private bool _isOverlayVisible = true;

        private string _overlayMessage = "Starting application. Please wait...";

        private string _serverUrl = string.Empty;

        public bool WebViewVisible
        {
            get { return _webViewVisible; }
            set
            {
                _webViewVisible = value;
                OnPropertyChange(nameof(WebViewVisible));
            }
        }

        public string ServerUrl
        {
            get { return _serverUrl; }
            set
            {
                _serverUrl = value;
                OnPropertyChange(nameof(ServerUrl));
            }
        }

        public bool IsOverlayVisible
        {
            get { return _isOverlayVisible; }
            set
            {
                _isOverlayVisible = value;
                OnPropertyChange(nameof(IsOverlayVisible));
            }
        }

        public string OverlayMessage
        {
            get { return _overlayMessage; }
            set
            {
                _overlayMessage = value;
                OnPropertyChange(nameof(OverlayMessage));
            }
        }

        public MainWindowViewModel(SyncController syncController)
        {
            _serverConnector = new Services.ServerConnector();
            _syncControllerModel = new SyncControllerViewModel(_serverConnector, () => ServerUrl = BusinessConstants.LocalServer.WebBaseUrl);
            syncController.DataContext = _syncControllerModel;
        }

        public async Task Init()
        {
            try
            {
                _syncControllerModel.ServerInitialized += InitializeWebView;
                _syncControllerModel.ShowServerMessage += ShowErrorMessage;
                _syncControllerModel.ShowSyncProgress += SyncProcessAction;
                await _syncControllerModel.Init();
            }
            catch (Exception ex)
            {
                OverlayMessage = $"There was an issue with the application. {ex.Message}";
            }
        }

        public void Dispose()
        {
            _syncControllerModel.Dispose();
        }

        private void ShowErrorMessage(object? sender, string e)
        {
            IsOverlayVisible = true;
            WebViewVisible = false;
            OverlayMessage = e;
        }

        private void InitializeWebView(object? sender, EventArgs e)
        {
            if (!IsOverlayVisible)
            {
                return;
            }

            ServerUrl = BusinessConstants.LocalServer.WebBaseUrl;
            IsOverlayVisible = false;
            WebViewVisible = true;
        }

        private void SyncProcessAction(object? sender, bool isStarted)
        {
            IsOverlayVisible = isStarted;
            WebViewVisible = !isStarted;
            OverlayMessage = "Sync with central server in progress. Please wait...";
        }
    }
}
