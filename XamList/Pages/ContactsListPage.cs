using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactsListPage : BaseContentPage<ContactsListViewModel>
    {
        public ContactsListPage()
        {
            var addContactButton = new ToolbarItem
            {
                Text = "+",
                AutomationId = AutomationIdConstants.AddContactButon
            };
            addContactButton.Clicked += HandleAddContactButtonClicked;
            ToolbarItems.Add(addContactButton);

            var contactsListView = new Xamarin.Forms.ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(ContactsListTextCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.Transparent,
                AutomationId = AutomationIdConstants.ContactsListView,
                RefreshControlColor = Color.Black
            };
            contactsListView.ItemTapped += HandleItemTapped;
            contactsListView.SetBinding(Xamarin.Forms.ListView.ItemsSourceProperty, nameof(ContactsListViewModel.AllContactsList));
            contactsListView.SetBinding(Xamarin.Forms.ListView.RefreshCommandProperty, nameof(ContactsListViewModel.RefreshCommand));
            contactsListView.SetBinding(Xamarin.Forms.ListView.IsRefreshingProperty, nameof(ContactsListViewModel.IsRefreshing));

            var restoreDeletedContactsButton = new Button
            {
                Text = "  Restore Deleted Contacts  ",
                TextColor = ColorConstants.TextColor,
                AutomationId = AutomationIdConstants.RestoreDeletedContactsButton,
                BackgroundColor = new Color(ColorConstants.NavigationBarBackgroundColor.R,
                                            ColorConstants.NavigationBarBackgroundColor.G,
                                            ColorConstants.NavigationBarBackgroundColor.B,
                                            0.25)
            };
            restoreDeletedContactsButton.Clicked += HandleRestoreDeletedContactsButtonClicked;

            Title = PageTitleConstants.ContactsListPage;

            var relativeLayout = new RelativeLayout();

            relativeLayout.Children.Add(contactsListView,
                                        Constraint.Constant(0),
                                        Constraint.Constant(0),
                                        Constraint.RelativeToParent(parent => parent.Width),
                                        Constraint.RelativeToParent(parent => parent.Height));
            relativeLayout.Children.Add(restoreDeletedContactsButton,
                                        Constraint.RelativeToParent(parent => parent.Width / 2 - getWidth(parent, restoreDeletedContactsButton) / 2),
                                        Constraint.RelativeToParent(parent => parent.Height - getHeight(parent, restoreDeletedContactsButton) - 10));

            Content = relativeLayout;

            On<iOS>().SetUseSafeArea(true);

            static double getHeight(RelativeLayout parent, View view) => view.Measure(parent.Width, parent.Height).Request.Height;
            static double getWidth(RelativeLayout parent, View view) => view.Measure(parent.Width, parent.Height).Request.Width;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AppCenterHelpers.TrackEvent(AppCenterConstants.ContactsListPageAppeared);

            if (Content is Layout<View> layout
                && layout.Children.OfType<Xamarin.Forms.ListView>().First() is Xamarin.Forms.ListView listView)
            {
                listView.BeginRefresh();
            }
        }

        async void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is Xamarin.Forms.ListView listView &&
                    e.Item is ContactModel selectedContactModel)
            {
                listView.SelectedItem = null;
                await Navigation.PushAsync(new ContactDetailPage(selectedContactModel, false));
            }
        }

        void HandleAddContactButtonClicked(object sender, EventArgs e)
        {
            AppCenterHelpers.TrackEvent(AppCenterConstants.AddContactButtonTapped);

            Device.BeginInvokeOnMainThread(async () =>
               await Navigation.PushModalAsync(new BaseNavigationPage(new ContactDetailPage(new ContactModel(), true))));
        }

        void HandleRestoreDeletedContactsButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var isDisplayAlertDialogConfirmed = await DisplayAlert("Restore Deleted Contacts",
                                                            "Would you like to restore deleted contacts?",
                                                            AlertDialogConstants.Yes,
                                                            AlertDialogConstants.Cancel);

                if (isDisplayAlertDialogConfirmed)
                    ViewModel.RestoreDeletedContactsCommand.Execute(null);
            });
        }
    }
}
