using CA.Ticketing.Desktop.ViewModels;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CA.Ticketing.Desktop
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel(syncController);
            this.DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)this.DataContext;
            Task.Run(() => viewModel.Init());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var viewModel = (MainWindowViewModel)this.DataContext;
            viewModel.Dispose();
            base.OnClosing(e);
        }
    }
}
