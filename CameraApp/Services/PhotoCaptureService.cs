using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using BitmapEncoder = Windows.Graphics.Imaging.BitmapEncoder;
using System.IO;

namespace CameraApp.Services
{
    public class PhotoCaptureService : IPhotoCaptureService
    {
        private readonly MediaCaptureInitializationSettings settings = new()
        {
            StreamingCaptureMode = StreamingCaptureMode.Video,
            MediaCategory = MediaCategory.Media
        };

        public async Task<MediaCapture> GetMediaCapture(string videoDeviceId)
        {
            var mediaCapture = new MediaCapture();
            settings.VideoDeviceId = videoDeviceId;
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
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, bitmapImageStream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();

                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = bitmapImageStream.AsStreamForRead();
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
