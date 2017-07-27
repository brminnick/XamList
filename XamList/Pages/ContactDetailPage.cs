using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

using XamList.Shared;
using XamList.Constants;

namespace XamList
{
    public class ContactDetailPage : BaseContentPage<ContactDetailViewModel>
    {
        #region Constant Fields
        readonly ContactModel _contactModel;
        ToolbarItem _saveToobarItem, _cancelToolbarItem;
        #endregion

        #region Constructors
        public ContactDetailPage(ContactModel selectedContact, bool canEdit)
        {
            _contactModel = selectedContact;
            BindingContext = _contactModel;

            var phoneNumberDataEntry = new ContactDetailEntry(canEdit)
            {
                ReturnType = ReturnType.Go,
                ReturnCommand = new Command(async () => await SaveContactAndPopPage()),
                AutomationId = AutomationIdConstants.PhoneNumberEntry
            };
            phoneNumberDataEntry.SetBinding(Entry.TextProperty, nameof(_contactModel.PhoneNumber));

            var lastNameDataEntry = new ContactDetailEntry(canEdit)
            {
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => phoneNumberDataEntry.Focus()),
                AutomationId = AutomationIdConstants.LastNameEntry
            };
            lastNameDataEntry.SetBinding(Entry.TextProperty, nameof(_contactModel.LastName));

            var firstNameDataEntry = new ContactDetailEntry(canEdit)
            {
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => lastNameDataEntry.Focus()),
                AutomationId = AutomationIdConstants.FirstNameEntry
            };
            firstNameDataEntry.SetBinding(Entry.TextProperty, nameof(_contactModel.FirstName));

            var phoneNumberTextLabel = new ContactDetailLabel { Text = "Phone Number" };
            var lastNameTextLabel = new ContactDetailLabel { Text = "Last Name" };
            var firstNameTextLabel = new ContactDetailLabel { Text = "First Name" };

            _saveToobarItem = new ToolbarItem
            {
                Text = "Save",
                Priority = 0,
                AutomationId = AutomationIdConstants.SaveContactButton
            };

            _cancelToolbarItem = new ToolbarItem
            {
                Text = "Cancel",
                Priority = 1,
                AutomationId = AutomationIdConstants.CancelContactButton
            };

            switch (canEdit)
            {
                case true:
                    ToolbarItems.Add(_saveToobarItem);
                    ToolbarItems.Add(_cancelToolbarItem);
                    break;
            }

            Title = PageTitles.ContactDetailsPage;

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
            MobileCenterHelpers.TrackEvent(MobileCenterConstants.ContactDetailPageAppeared);
        }

        protected override void SubscribeEventHandlers()
        {
            _cancelToolbarItem.Clicked += HandleCancelToolBarItemClicked;
            _saveToobarItem.Clicked += HandleSaveToolbarItemClicked;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _cancelToolbarItem.Clicked -= HandleCancelToolBarItemClicked;
            _saveToobarItem.Clicked -= HandleSaveToolbarItemClicked;
        }

        async Task SaveContactAndPopPage()
        {
            await ContactDatabase.SaveContact(_contactModel);
            PopPage();
        }

        async void HandleSaveToolbarItemClicked(object sender, EventArgs e) =>
            await SaveContactAndPopPage();

        void HandleCancelToolBarItemClicked(object sender, EventArgs e) => PopPage();
        void PopPage() => Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
        #endregion

        #region Classes
        class ContactDetailEntry : CustomReturnEntry
        {
            public ContactDetailEntry(bool canEdit)
            {
                IsEnabled = canEdit;
                TextColor = Color.FromHex("2B3E50");
            }
        }

        class ContactDetailLabel : Label
        {
            public ContactDetailLabel() => TextColor = Color.FromHex("1B2A38");
        }
        #endregion
    }
}
