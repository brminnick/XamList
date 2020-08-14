using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Xamarin.Essentials.Interfaces;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactDetailViewModel : BaseViewModel
    {
        readonly ApiService _apiService;
        readonly IMainThread _mainThread;
        readonly ContactDatabase _contactDatabase;
        readonly WeakEventManager<bool> _saveContactCompletedEventManager = new WeakEventManager<bool>();

        bool _isSaving;
        ContactModel _contact = new ContactModel();

        public ContactDetailViewModel(ApiService apiService, ContactDatabase contactDatabase, IMainThread mainThread, AppCenterService appCenterService) : base(appCenterService)
        {
            _apiService = apiService;
            _mainThread = mainThread;
            _contactDatabase = contactDatabase;

            SaveButtonTappedCommand = new AsyncCommand<bool>(ExecuteSaveButtonTappedCommand, _ => !IsSaving);
        }

        public event EventHandler<bool> SaveContactCompleted
        {
            add => _saveContactCompletedEventManager.AddEventHandler(value);
            remove => _saveContactCompletedEventManager.RemoveEventHandler(value);
        }

        public IAsyncCommand<bool> SaveButtonTappedCommand { get; }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value, () => _mainThread.BeginInvokeOnMainThread(SaveButtonTappedCommand.RaiseCanExecuteChanged));
        }

        public string FirstNameText
        {
            get => Contact.FirstName;
            set
            {
                Contact.FirstName = value;
                NotifyContactProperties();
            }
        }

        public string LastNameText
        {
            get => Contact.LastName;
            set
            {
                Contact.LastName = value;
                NotifyContactProperties();
            }
        }

        public string PhoneNumberText
        {
            get => Contact.PhoneNumber;
            set
            {
                Contact.PhoneNumber = value;
                NotifyContactProperties();
            }
        }

        public ContactModel Contact
        {
            get => _contact;
            set => SetProperty(ref _contact, value, NotifyContactProperties);
        }

        void NotifyContactProperties()
        {
            OnPropertyChanged(nameof(FirstNameText));
            OnPropertyChanged(nameof(LastNameText));
            OnPropertyChanged(nameof(PhoneNumberText));
        }

        async Task ExecuteSaveButtonTappedCommand(bool isNewContact)
        {
            IsSaving = true;

            try
            {
                var saveToRemoteDatabaseTask = isNewContact switch
                {
                    true => _apiService.PostContactModel(Contact),
                    false => _apiService.PatchContactModel(Contact)
                };

                var saveToLocalDatabaseTask = _contactDatabase.SaveContact(Contact);

                await Task.WhenAll(saveToRemoteDatabaseTask, saveToLocalDatabaseTask).ConfigureAwait(false);

                OnSaveContactCompleted(true);
            }
            catch
            {
                OnSaveContactCompleted(false);
            }
            finally
            {
                IsSaving = false;
            }
        }

        void OnSaveContactCompleted(bool isSaveSuccessful) =>
            _saveContactCompletedEventManager.RaiseEvent(this, isSaveSuccessful, nameof(SaveContactCompleted));
    }
}
