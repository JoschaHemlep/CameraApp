using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;
using System.Windows.Input;
using CameraApp.Common;
using CameraApp.Services;

namespace CameraApp.Views
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private DeviceInformation selectedCamera;
        private DeviceInformationCollection cameras;
        private BitmapImage photo;
        private int selectedCameraIndex;
        private readonly IPhotoCaptureService photoCaptureService;

        public IConfiguration Configuration { get; }

        /// <inheritdoc />
        public DeviceInformation SelectedCamera
        {
            get => selectedCamera; set
            {
                SetAndNotifyIfChanged(ref selectedCamera, value);
                CapturePhotoCommand.RaiseCanExecuteChanged();
            }
        }

        /// <inheritdoc />
        public DeviceInformationCollection Cameras
        {
            get => cameras; set
            {
                SetAndNotifyIfChanged(ref cameras, value);
                SwitchCameraCommand.RaiseCanExecuteChanged();
            }
        }

        public BitmapImage Photo
        {
            get => photo; set
            {
                if (photo != value)
                {
                    photo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IAsyncCommand CapturePhotoCommand { get; set; }
        public IAsyncCommand SwitchCameraCommand { get; set; }

        ICommand IMainViewModel.CapturePhotoCommand => CapturePhotoCommand;

        ICommand IMainViewModel.SwitchCameraCommand => SwitchCameraCommand;

        public MainViewModel(IPhotoCaptureService photoCaptureService, IConfiguration configuration)
        {
            this.photoCaptureService = photoCaptureService;
            Configuration = configuration;
            CapturePhotoCommand = new AsyncCommand(CapturePhoto, CanCapturePhotoCommand);
            SwitchCameraCommand = new AsyncCommand(SwitchCamera, CanSwitchCameraCommand);
        }

        private bool CanSwitchCameraCommand()
        {
            return Cameras != null && Cameras.Count > 1;
        }

        private bool CanCapturePhotoCommand()
        {
            return SelectedCamera != null;
        }

        private Task SwitchCamera(object parameter)
        {
            var newSelectedIndex = selectedCameraIndex + 1;
            if(Cameras.Count < newSelectedIndex + 1)
            {
                newSelectedIndex = 0;
            }
            SelectedCamera = Cameras[newSelectedIndex];
            selectedCameraIndex = newSelectedIndex;

            return Task.CompletedTask;
        }

        private async Task CapturePhoto(object parameter)
        {
            var photo = await CapturePhoto();

            Photo = photo;
        }

        /// <inheritdoc />
        public async Task Initialize()
        {
            Cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (Cameras.Count == 0)
            {
                return;
            }

            selectedCameraIndex = 0;

            try
            {
                selectedCameraIndex = int.Parse(Configuration["SelectedCamera"]);
                if(selectedCameraIndex > Cameras.Count - 1)
                {
                    selectedCameraIndex = 0;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading app settings");
            }

            SelectedCamera = Cameras[selectedCameraIndex];
        }


        public async Task<BitmapImage> CapturePhoto()
        {
            if (SelectedCamera == null)
            {
                throw new NotSupportedException("No camera selected");
            }

            using (var mediaCapture = await photoCaptureService.GetMediaCapture(SelectedCamera.Id))
            {
                var photo = await photoCaptureService.CapturePhoto(mediaCapture);
                var bitmapImage = await photoCaptureService.ToBitmapImage(photo);
                return bitmapImage;
            }
        }


    }
}