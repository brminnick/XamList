using System;

using Xamarin.Forms;

using XamList.Shared;
using XamList.Mobile.Shared;

namespace XamList
{
    public class ContactDetailPage : BaseContentPage<ContactDetailViewModel>
    {
        #region Constant Fields
        readonly bool _isNewContact;
        #endregion

        #region Constructors
        public ContactDetailPage(ContactModel selectedContact, bool isNewContact)
        {
            ViewModel.Contact = selectedContact;
            ViewModel.SaveContactCompleted += HandleSaveContactCompleted;

            _isNewContact = isNewContact;

            var phoneNumberDataEntry = new ContactDetailEntry
            {
                ReturnType = ReturnType.Go,
                AutomationId = AutomationIdConstants.PhoneNumberEntry,
                ReturnCommand = new Command(Unfocus)
            };
            phoneNumberDataEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.PhoneNumberText));

            var lastNameDataEntry = new ContactDetailEntry
            {
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => phoneNumberDataEntry.Focus()),
                AutomationId = AutomationIdConstants.LastNameEntry
            };
            lastNameDataEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.LastNameText));

            var firstNameDataEntry = new ContactDetailEntry
            {
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => lastNameDataEntry.Focus()),
                AutomationId = AutomationIdConstants.FirstNameEntry
            };
            firstNameDataEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.FirstNameText));

            var phoneNumberTextLabel = new ContactDetailLabel { Text = "Phone Number" };
            var lastNameTextLabel = new ContactDetailLabel { Text = "Last Name" };
            var firstNameTextLabel = new ContactDetailLabel { Text = "First Name" };

            var isSavingIndicator = new ActivityIndicator();
            isSavingIndicator.SetBinding(IsVisibleProperty, nameof(ViewModel.IsSaving));
            isSavingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsSaving));

            var saveToobarItem = new ToolbarItem
            {
                Text = "Save",
                Priority = 0,
                AutomationId = AutomationIdConstants.SaveContactButton,
                CommandParameter = _isNewContact
            };
            saveToobarItem.SetBinding(MenuItem.CommandProperty, nameof(ViewModel.SaveButtonTappedCommand));

            var cancelToolbarItem = new ToolbarItem
            {
                Text = "Cancel",
                Priority = 1,
                AutomationId = AutomationIdConstants.CancelContactButton
            };
            cancelToolbarItem.Clicked += HandleCancelToolBarItemClicked;

            ToolbarItems.Add(saveToobarItem);

            if (isNewContact)
                ToolbarItems.Add(cancelToolbarItem);


            Title = PageTitleConstants.ContactDetailsPage;

            Padding = new Thickness(20, 0, 20, 0);

            Content = new StackLayout
            {
                Margin = new Thickness(0, 10, 0, 0),
                Children = {
                    firstNameTextLabel,
                    firstNameDataEntry,
                    lastNameTextLabel,
                    lastNameDataEntry,
                    phoneNumberTextLabel,
                    phoneNumberDataEntry,
                    isSavingIndicator
                }
            };
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            AppCenterHelpers.TrackEvent(AppCenterConstants.ContactDetailPageAppeared);
        }

        void PopPage()
        {
            if (_isNewContact)
                Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
            else
                Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
        }

        void HandleSaveContactCompleted(object sender, EventArgs e) => PopPage();

        void HandleCancelToolBarItemClicked(object sender, EventArgs e) => PopPage();
        #endregion

        #region Classes
        class ContactDetailEntry : Entry
        {
            public ContactDetailEntry() => TextColor = Color.FromHex("2B3E50");
        }

        class ContactDetailLabel : Label
        {
            public ContactDetailLabel() => TextColor = Color.FromHex("1B2A38");
        }
        #endregion
    }
}
