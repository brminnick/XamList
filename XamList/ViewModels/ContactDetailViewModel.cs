using System;
using System.Windows.Input;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

using XamList.Shared;
using XamList.Mobile.Shared;

namespace XamList
{
    public class ContactDetailViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly WeakEventManager _saveContactCompletedEventManager = new WeakEventManager();
        #endregion

        #region Fields
        bool _isSaving;
        ContactModel _contact;
        ICommand _saveButtonTappedCommand;
        #endregion

        #region Events
        public event EventHandler SaveContactCompleted
        {
            add => _saveContactCompletedEventManager.AddEventHandler(value);
            remove => _saveContactCompletedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        public ICommand SaveButtonTappedCommand => _saveButtonTappedCommand ??
            (_saveButtonTappedCommand = new AsyncCommand<bool>(ExecuteSaveButtonTappedCommand));

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
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
        #endregion

        #region Methods
        void NotifyContactProperties()
        {
            OnPropertyChanged(nameof(FirstNameText));
            OnPropertyChanged(nameof(LastNameText));
            OnPropertyChanged(nameof(PhoneNumberText));
        }

        async Task ExecuteSaveButtonTappedCommand(bool isNewContact)
        {
            if (!IsSaving)
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

                    OnSaveContactCompleted();
                }
                finally
                {
                    IsSaving = false;
                }
            }
        }

        void OnSaveContactCompleted() => _saveContactCompletedEventManager.HandleEvent(this, EventArgs.Empty, nameof(SaveContactCompleted));
        #endregion
    }
}
