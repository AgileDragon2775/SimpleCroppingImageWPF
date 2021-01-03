using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace CustomImageChange
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual T Get<T>(ref T member) where T : new()
        {
            if (member == null)
            {
                member = new T();
            }

            return member;
        }

        protected virtual void Set<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(member, val))
            {
                member = val;
                RaisePropertyChanged(propertyName);
            }
        }
    }
}
