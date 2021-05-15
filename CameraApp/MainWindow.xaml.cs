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
        public IConfiguration Configuration { get; }

        public MainWindow(IConfiguration configuration)
        {
            Configuration = configuration;

            InitializeComponent();

            ReadAllSettings();
        }

        private void ReadAllSettings()
        {
            try
            {
                var connectionString = Configuration.GetConnectionString("MyDatabase");

                MyTextBlock.Text += "connectionString: " + connectionString + "\n";
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading app settings");
                MyTextBlock.Text += "Error reading app settings.\n";
            }
        }
    }
}