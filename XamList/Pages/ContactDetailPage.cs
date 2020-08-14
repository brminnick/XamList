using System;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
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

            ToolbarItems.Add(new ToolbarItem { Text = "Save", Priority = 0, AutomationId = AutomationIdConstants.SaveContactButton, CommandParameter = _isNewContact }
                                .Bind(MenuItem.CommandProperty, nameof(ContactDetailViewModel.SaveButtonTappedCommand)));

            if (isNewContact)
            {
                ToolbarItems.Add(new ToolbarItem { Text = "Cancel", Priority = 1, AutomationId = AutomationIdConstants.CancelContactButton }
                                    .Invoke(toolBarItem => toolBarItem.Clicked += HandleCancelToolBarItemClicked));
            }

            Title = PageTitleConstants.ContactDetailsPage;
            Padding = new Thickness(20, 0, 20, 0);

            Content = new StackLayout
            {
                Margin = new Thickness(0, 10, 0, 0),

                Children =
                {
                    new ContactDetailLabel { Text = "First Name" },

                    new ContactDetailEntry { ReturnType = ReturnType.Next, AutomationId = AutomationIdConstants.FirstNameEntry }
                      .Bind(Entry.TextProperty, nameof(ContactDetailViewModel.FirstNameText)),

                    new ContactDetailLabel { Text = "Last Name" },

                    new ContactDetailEntry { ReturnType = ReturnType.Next, AutomationId = AutomationIdConstants.LastNameEntry }
                        .Bind(Entry.TextProperty, nameof(ContactDetailViewModel.LastNameText)),

                    new ContactDetailLabel { Text = "Phone Number" },

                    new ContactDetailEntry { AutomationId = AutomationIdConstants.PhoneNumberEntry }
                        .Bind(Entry.TextProperty, nameof(ContactDetailViewModel.PhoneNumberText)),

                    new ActivityIndicator()
                        .Bind(IsVisibleProperty, nameof(ContactDetailViewModel.IsSaving))
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(ContactDetailViewModel.IsSaving))
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
