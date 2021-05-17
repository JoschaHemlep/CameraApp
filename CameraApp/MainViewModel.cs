using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using BitmapEncoder = Windows.Graphics.Imaging.BitmapEncoder;
using System.IO;

namespace CameraApp
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private DeviceInformation selectedCamera;
        private DeviceInformationCollection cameras;

        public IConfiguration Configuration { get; }

        /// <inheritdoc />
        public DeviceInformation SelectedCamera { get => selectedCamera; set => SetAndNotifyIfChanged(ref selectedCamera, value); }

        /// <inheritdoc />
        public DeviceInformationCollection Cameras { get => cameras; set => SetAndNotifyIfChanged(ref cameras, value); }

        public MainViewModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            Cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (Cameras.Count == 0)
            {
                return;
            }

            SelectedCamera = Cameras[0];
        }

        public async Task<BitmapImage> CapturePhoto()
        {
            if (SelectedCamera == null)
            {
                throw new NotSupportedException("No camera selected");
            }

            var mediaCapture = await GetMediaCapture();
            var photo = await CapturePhoto(mediaCapture);
            var bitmapImage = await ToBitmapImage(photo);

            return bitmapImage;
        }

        public async Task<MediaCapture> GetMediaCapture()
        {
            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = SelectedCamera.Id
            };

            var mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(settings);

            return mediaCapture;
        }

        public async Task<SoftwareBitmap> CapturePhoto(MediaCapture mediaCapture)
        {
            using (var captureStream = new InMemoryRandomAccessStream())
            {
                await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateBmp(), captureStream);

                var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

                var capturedPhoto = await lowLagCapture.CaptureAsync();
                var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;
                await lowLagCapture.FinishAsync();

                return softwareBitmap;
            }
        }

        public async Task<BitmapImage> ToBitmapImage(SoftwareBitmap softwareBitmap)
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
    }
}