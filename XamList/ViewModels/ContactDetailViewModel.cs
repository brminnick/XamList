using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

using XamList.Shared;

namespace XamList
{
    public class ContactDetailViewModel : BaseViewModel
    {
        #region Fields
        ContactModel _contact;
        Command<bool> _saveButtonTappedCommand;
        #endregion

        #region Events
        public event EventHandler SaveContactCompleted;
        #endregion

        #region Properties
        public Command<bool> SaveButtonTappedCommand => _saveButtonTappedCommand ??
            (_saveButtonTappedCommand = new Command<bool>(async (isNewContact) => await ExecuteSaveButtonTappedCommand(isNewContact)));

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

            switch (isNewContact)
            {
                case true:
                    saveContactTaskList.Add(APIService.PostContactModel(Contact));
                    break;

                default:
                    saveContactTaskList.Add(APIService.PatchContactModel(Contact));
                    break;
            }

            await Task.WhenAll(saveContactTaskList).ConfigureAwait(false);

            OnSaveContactCompleted();
        }

        void OnSaveContactCompleted() => SaveContactCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
