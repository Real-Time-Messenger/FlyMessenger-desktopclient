using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlyMessenger.Core
{
    /// <summary>
    /// Observable object.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        // Property changed event.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Invoke property changed event.
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
