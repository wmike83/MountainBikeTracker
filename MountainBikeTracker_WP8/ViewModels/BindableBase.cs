using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MountainBikeTracker_WP8.ViewModels
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                App.RootFrame.Dispatcher.BeginInvoke(() =>
                {
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }
}
