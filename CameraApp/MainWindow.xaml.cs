using Microsoft.Extensions.Configuration;
using System;
using System.Windows;

namespace CameraApp
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

                MyTextBlock.Text += $"connectionString: {connectionString}\n";
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading app settings");
                MyTextBlock.Text += "Error reading app settings.\n";
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Initialize();

            // ToDo: Change connectionstring in selected camera
            ReadConnectionstring();
            try
            {
                var photo = await ViewModel.CapturePhoto();

                Dispatcher.Invoke(() =>
                {
                    MyImage.Source = photo;
                });
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    caption: "Error",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
            }
        }
    }
}