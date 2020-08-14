using System;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactDetailPage : BaseContentPage<ContactDetailViewModel>
    {
        readonly bool _isNewContact;
        readonly IMainThread _mainThread;

        public ContactDetailPage(bool isNewContact,
                                    IMainThread mainThread,
                                    ContactModel selectedContact,
                                    AppCenterService appCenterService,
                                    ContactDetailViewModel contactDetailViewModel) : base(contactDetailViewModel, appCenterService)
        {
            _mainThread = mainThread;
            _isNewContact = isNewContact;

            ViewModel.Contact = selectedContact;
            ViewModel.SaveContactCompleted += HandleSaveContactCompleted;


            var firstNameTextLabel = new ContactDetailLabel { Text = "First Name" };

            var firstNameDataEntry = new ContactDetailEntry
            {
                ReturnType = ReturnType.Next,
                AutomationId = AutomationIdConstants.FirstNameEntry
            };
            firstNameDataEntry.SetBinding(Entry.TextProperty, nameof(ContactDetailViewModel.FirstNameText));

            var lastNameTextLabel = new ContactDetailLabel { Text = "Last Name" };

            var lastNameDataEntry = new ContactDetailEntry
            {
                ReturnType = ReturnType.Next,
                AutomationId = AutomationIdConstants.LastNameEntry
            };
            lastNameDataEntry.SetBinding(Entry.TextProperty, nameof(ContactDetailViewModel.LastNameText));

            var phoneNumberTextLabel = new ContactDetailLabel { Text = "Phone Number" };

            var phoneNumberDataEntry = new ContactDetailEntry
            {
                AutomationId = AutomationIdConstants.PhoneNumberEntry
            };
            phoneNumberDataEntry.SetBinding(Entry.TextProperty, nameof(ContactDetailViewModel.PhoneNumberText));

            var isSavingIndicator = new ActivityIndicator();
            isSavingIndicator.SetBinding(IsVisibleProperty, nameof(ContactDetailViewModel.IsSaving));
            isSavingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ContactDetailViewModel.IsSaving));

            var saveToobarItem = new ToolbarItem
            {
                Text = "Save",
                Priority = 0,
                AutomationId = AutomationIdConstants.SaveContactButton,
                CommandParameter = _isNewContact
            };
            saveToobarItem.SetBinding(MenuItem.CommandProperty, nameof(ContactDetailViewModel.SaveButtonTappedCommand));

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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AppCenterService.Track(AppCenterConstants.ContactDetailPageAppeared);
        }

        Task PopPage()
        {
            if (_isNewContact)
                return _mainThread.InvokeOnMainThreadAsync(() => Navigation.PopModalAsync());

            return _mainThread.InvokeOnMainThreadAsync(() => Navigation.PopAsync());
        }

        async void HandleSaveContactCompleted(object sender, bool isSaveSuccessful)
        {
            if (isSaveSuccessful)
                await PopPage();
            else
                await DisplayAlert("Save Failed", "", "OK");
        }

        async void HandleCancelToolBarItemClicked(object sender, EventArgs e) => await PopPage();

        class ContactDetailEntry : Entry
        {
            public ContactDetailEntry()
            {
                TextColor = Color.FromHex("2B3E50");
                BackgroundColor = Color.White;
            }
        }

        class ContactDetailLabel : Label
        {
            public ContactDetailLabel() => TextColor = Color.FromHex("1B2A38");
        }
    }
}
