using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactDetailViewModel : BaseViewModel
    {
        readonly WeakEventManager<bool> _saveContactCompletedEventManager = new WeakEventManager<bool>();

        bool _isSaving;
        ContactModel _contact = new ContactModel();
        IAsyncCommand<bool>? _saveButtonTappedCommand;

        public event EventHandler<bool> SaveContactCompleted
        {
            add => _saveContactCompletedEventManager.AddEventHandler(value);
            remove => _saveContactCompletedEventManager.RemoveEventHandler(value);
        }

        public IAsyncCommand<bool> SaveButtonTappedCommand => _saveButtonTappedCommand ??= new AsyncCommand<bool>(ExecuteSaveButtonTappedCommand, _ => !IsSaving);

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value, () => Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(SaveButtonTappedCommand.RaiseCanExecuteChanged));
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

                Task<ContactModel> saveToRemoteDatabaseTask;

                var saveToLocalDatabaseTask = ContactDatabase.SaveContact(Contact);

                if (isNewContact)
                    saveToRemoteDatabaseTask = ApiService.PostContactModel(Contact);
                else
                    saveToRemoteDatabaseTask = ApiService.PatchContactModel(Contact);

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
            _saveContactCompletedEventManager.HandleEvent(this, isSaveSuccessful, nameof(SaveContactCompleted));
    }
}
