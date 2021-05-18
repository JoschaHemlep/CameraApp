using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;

namespace CameraApp.Views
{
    /// <summary>
    /// ViewModel for <see cref="MainWindow"/>
    /// </summary>
    public interface IMainViewModel
    {
        IConfiguration Configuration { get; }

        /// <summary>
        /// All available cameras
        /// </summary>
        DeviceInformationCollection Cameras { get; }

        /// <summary>
        /// The selected camera
        /// </summary>
        DeviceInformation SelectedCamera { get; set; }

        /// <summary>
        /// Initializes the ViewModel
        /// </summary>
        Task Initialize();

        BitmapImage Photo { get; }

        ICommand CapturePhotoCommand { get; }
        ICommand SwitchCameraCommand { get; }
    }
}