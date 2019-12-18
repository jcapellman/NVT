using MahApps.Metro.Controls;

using NCAT.ViewModels;

namespace NCAT
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}