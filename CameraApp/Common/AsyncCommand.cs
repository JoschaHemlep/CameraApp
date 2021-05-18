using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CameraApp.Common
{
    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<object, Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncCommand(
            Func<object, Task> execute,
            Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        async void ICommand.Execute(object parameter)
        {
            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception)
            {
                // ToDo: Handle Exceptions
            }
        }
    }
}