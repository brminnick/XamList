using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using AsyncAwaitBestPractices.MVVM;

using XamList.Shared;

namespace XamList
{
    public class ContactDetailViewModel : BaseViewModel
    {
        #region Fields
        ContactModel _contact;
        ICommand _saveButtonTappedCommand;
        #endregion

        #region Events
        public event EventHandler SaveContactCompleted;
        #endregion

        #region Properties
        public ICommand SaveButtonTappedCommand => _saveButtonTappedCommand ??
            (_saveButtonTappedCommand = new AsyncCommand<bool>(ExecuteSaveButtonTappedCommand, continueOnCapturedContext: false));

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
            var saveContactTaskList = new List<Task> { ContactDatabase.SaveContact(Contact) };

            if (isNewContact)
                saveContactTaskList.Add(APIService.PostContactModel(Contact));
            else
                saveContactTaskList.Add(APIService.PatchContactModel(Contact));

            await Task.WhenAll(saveContactTaskList).ConfigureAwait(false);

            OnSaveContactCompleted();
        }

        void OnSaveContactCompleted() => SaveContactCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
