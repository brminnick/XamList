using System;

using Xamarin.Forms;

using XamList.Mobile.Common;
using XamList.Shared;

namespace XamList
{
    public class ContactsListPage : BaseContentPage<ContactsListViewModel>
    {
        #region Constant Fields
        readonly ListView _contactsListView;
        readonly ToolbarItem _addContactButton;
        readonly Button _restoreDeletedContactsButton;
        #endregion

        #region Constructors
        public ContactsListPage()
        {
            _addContactButton = new ToolbarItem
            {
                Text = "+",
                AutomationId = AutomationIdConstants.AddContactButon
            };
            ToolbarItems.Add(_addContactButton);

            _contactsListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(ContactsListTextCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.Transparent
            };
            _contactsListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.AllContactsList));
            _contactsListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            _restoreDeletedContactsButton = new Button
            {
                Text = "  Restore Deleted Contacts  ",
                TextColor = ColorConstants.TextColor,
				AutomationId = AutomationIdConstants.RestoreDeletedContactsButton,
                BackgroundColor = new Color(ColorConstants.NavigationBarBackgroundColor.R,
                                            ColorConstants.NavigationBarBackgroundColor.G,
                                            ColorConstants.NavigationBarBackgroundColor.B,
                                            0.25)
            };

            Title = PageTitles.ContactsListPage;

            var relativeLayout = new RelativeLayout();

            Func<RelativeLayout, double> getRestoreDeletedContactsButtonHeight = parent => _restoreDeletedContactsButton.Measure(parent.Width, parent.Height).Request.Height;
            Func<RelativeLayout, double> getRestoreDeletedContactsButtonWidth = parent => _restoreDeletedContactsButton.Measure(parent.Width, parent.Height).Request.Width;

            relativeLayout.Children.Add(_contactsListView,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0),
                                       Constraint.RelativeToParent(parent => parent.Width),
                                       Constraint.RelativeToParent(parent => parent.Height));
            relativeLayout.Children.Add(_restoreDeletedContactsButton,
                                        Constraint.RelativeToParent(parent => parent.Width / 2 - getRestoreDeletedContactsButtonWidth(parent) / 2),
                                        Constraint.RelativeToParent(parent => parent.Height - getRestoreDeletedContactsButtonHeight(parent) - 10));

            Content = relativeLayout;
        }
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MobileCenterHelpers.TrackEvent(MobileCenterConstants.ContactsListPageAppeared);

            Device.BeginInvokeOnMainThread(_contactsListView.BeginRefresh);
        }

        protected override void SubscribeEventHandlers()
        {
            _contactsListView.ItemSelected += HandleItemSelected;
            _addContactButton.Clicked += HandleAddContactButtonClicked;
            ViewModel.PullToRefreshCompleted += HandlePullToRefreshCompleted;
            ViewModel.RestoreDeletedContactsCompleted += HandleRestoreDeletedContactsCompleted;
            _restoreDeletedContactsButton.Clicked += HandleRestoreDeletedContactsButtonClicked;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _contactsListView.ItemSelected -= HandleItemSelected;
            _addContactButton.Clicked -= HandleAddContactButtonClicked;
            ViewModel.PullToRefreshCompleted -= HandlePullToRefreshCompleted;
            ViewModel.RestoreDeletedContactsCompleted -= HandleRestoreDeletedContactsCompleted;
			_restoreDeletedContactsButton.Clicked -= HandleRestoreDeletedContactsButtonClicked;
        }

        void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selectedContactModel = e?.SelectedItem as ContactModel;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new ContactDetailPage(selectedContactModel, false));
                listView.SelectedItem = null;
            });
        }

        void HandleAddContactButtonClicked(object sender, EventArgs e)
        {
            MobileCenterHelpers.TrackEvent(MobileCenterConstants.AddContactButtonTapped);

            Device.BeginInvokeOnMainThread(async () =>
               await Navigation.PushModalAsync(new BaseNavigationPage(new ContactDetailPage(new ContactModel(), true))));
        }

        async void HandleRestoreDeletedContactsButtonClicked(object sender, EventArgs e)
        {
            var isDisplayAlertDialogConfirmed = await DisplayAlert("Restore Deleted Contacts", 
                                                        "Would you like to restore deleted contacts?", 
                                                        AlertDialogConstants.Yes, 
                                                        AlertDialogConstants.Cancel);

            switch(isDisplayAlertDialogConfirmed)
            {
                case true:
                    ViewModel.RestoreDeletedContactsCommand?.Execute(null);
                    break;
            }
        }


        void HandleRestoreDeletedContactsCompleted(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(_contactsListView.BeginRefresh);

        public void HandlePullToRefreshCompleted(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(_contactsListView.EndRefresh);
        #endregion
    }
}
