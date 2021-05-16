using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using BitmapEncoder = Windows.Graphics.Imaging.BitmapEncoder;

namespace CameraApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InMemoryRandomAccessStream bitmapImageStream;

        public IConfiguration Configuration { get; }

        public MainWindow(IConfiguration configuration)
        {
            Configuration = configuration;

            InitializeComponent();

            // ToDo: Change connectionstring in selected camera
            ReadConnectionstring();
        }

        
        private void ReadConnectionstring()
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var camera = await GetCamera();
            var photo = await CapturePhoto(camera);
            var bitmapImage = await ToBitmapImage(photo);

            Dispatcher.Invoke(() =>
            {
                MyImage.Source = bitmapImage;
            });
        }

        private async Task<MediaCapture> GetCamera()
        {
            var video = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            var settings = new MediaCaptureInitializationSettings
            {
                // select the first video device - ToDo: let the user select the camera + save selected device in appSettings
                VideoDeviceId = video[0].Id
            };

            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(settings);
            mediaCapture.Failed += MediaCapture_Failed;

            return mediaCapture;
        }

        private async Task<SoftwareBitmap> CapturePhoto(MediaCapture mediaCapture)
        {
            using (var captureStream = new InMemoryRandomAccessStream())
            {
                // capture photo
                await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateBmp(), captureStream);

                var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

                var capturedPhoto = await lowLagCapture.CaptureAsync();
                var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;
                await lowLagCapture.FinishAsync();

                return softwareBitmap;
            }
        }

        private async Task<BitmapImage> ToBitmapImage(SoftwareBitmap softwareBitmap)
        {
            using (var bitmapImageStream = new InMemoryRandomAccessStream())
            {

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, bitmapImageStream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();

                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = bitmapImageStream.AsStream();
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            MessageBox.Show("Failed to initalize");
        }
    }

}