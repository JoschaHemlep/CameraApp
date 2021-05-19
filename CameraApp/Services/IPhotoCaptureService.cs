using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;

namespace CameraApp.Services
{
    public interface IPhotoCaptureService
    {
        Task<MediaCapture> GetMediaCapture(string videoDeviceId);

        Task<SoftwareBitmap> CapturePhoto(MediaCapture mediaCapture);

        Task<BitmapImage> ToBitmapImage(SoftwareBitmap softwareBitmap);
    }
}
