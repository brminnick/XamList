using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;

namespace XamList
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        readonly WeakEventManager _propertyChangedEventManager = new WeakEventManager();

        public BaseViewModel(AppCenterService appCenterService) => AppCenterService = appCenterService;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _propertyChangedEventManager.AddEventHandler(value);
            remove => _propertyChangedEventManager.RemoveEventHandler(value);
        }

        protected AppCenterService AppCenterService { get; }

        protected void SetProperty<T>(ref T backingStore, in T value, in Action? onChanged = null, [CallerMemberName] in string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        protected void OnPropertyChanged([CallerMemberName] in string propertyName = "") =>
            _propertyChangedEventManager.RaiseEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));
    }
}
