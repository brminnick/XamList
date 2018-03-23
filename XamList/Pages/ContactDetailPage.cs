using System;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

using XamList.Shared;

namespace XamList
{
    public class ContactDetailPage : BaseContentPage<ContactDetailViewModel>
    {
        #region Constant Fields
        readonly bool _isNewContact;
        ToolbarItem _saveToobarItem, _cancelToolbarItem;
        #endregion

        #region Constructors
        public ContactDetailPage(ContactModel selectedContact, bool isNewContact)
        {
            _isNewContact = isNewContact;
            ViewModel.Contact = selectedContact;

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

            _saveToobarItem = new ToolbarItem
            {
                Text = "Save",
                Priority = 0,
                AutomationId = AutomationIdConstants.SaveContactButton,
                CommandParameter = _isNewContact
            };
            _saveToobarItem.SetBinding(MenuItem.CommandProperty, nameof(ViewModel.SaveButtonTappedCommand));

            _cancelToolbarItem = new ToolbarItem
            {
                Text = "Cancel",
                Priority = 1,
                AutomationId = AutomationIdConstants.CancelContactButton
            };

            ToolbarItems.Add(_saveToobarItem);

            switch (isNewContact)
            {
                case true:
                    ToolbarItems.Add(_cancelToolbarItem);
                    break;
            }

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
                    phoneNumberDataEntry
                }
            };
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            AppCenterHelpers.TrackEvent(MobileCenterConstants.ContactDetailPageAppeared);
        }

        protected override void SubscribeEventHandlers()
        {
            _cancelToolbarItem.Clicked += HandleCancelToolBarItemClicked;
            ViewModel.SaveContactCompleted += HandleSaveContactCompleted;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _cancelToolbarItem.Clicked -= HandleCancelToolBarItemClicked;
            ViewModel.SaveContactCompleted -= HandleSaveContactCompleted;
        }

        void PopPage()
        {
            switch (_isNewContact)
            {
                case true:
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
                    break;

                default:
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync());
                    break;
            }
        }

        void HandleSaveContactCompleted(object sender, EventArgs e) => PopPage();

        void HandleCancelToolBarItemClicked(object sender, EventArgs e) => PopPage();
        #endregion

        #region Classes
        class ContactDetailEntry : CustomReturnEntry
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
