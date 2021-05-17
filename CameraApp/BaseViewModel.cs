using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CameraApp
{
    /// <summary>
    /// Base class for ViewModels
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetAndNotifyIfChanged<T>(ref T currentValue, T newValue) where T : IEquatable<T>
        {
            if (!Equals(currentValue, newValue))
            {
                currentValue = newValue;
                NotifyPropertyChanged();
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}