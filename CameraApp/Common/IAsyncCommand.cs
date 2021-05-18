using System.Threading.Tasks;
using System.Windows.Input;

namespace CameraApp.Common
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);

        bool CanExecute();
    }
}