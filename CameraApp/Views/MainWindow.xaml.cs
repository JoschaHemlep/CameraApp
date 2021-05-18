using Microsoft.Extensions.Configuration;
using System;
using System.Windows;

namespace CameraApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IMainViewModel ViewModel { get; }

        public MainWindow(IMainViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        private void ReadConnectionstring()
        {
            try
            {
                var connectionString = ViewModel.Configuration.GetConnectionString("MyDatabase");
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Initialize();

            // ToDo: Change connectionstring in selected camera
            ReadConnectionstring();
        }
    }
}