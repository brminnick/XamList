using System;
using System.Linq;
using Autofac;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactsListPage : BaseContentPage<ContactsListViewModel>
    {
        readonly IMainThread _mainThread;

        public ContactsListPage(IMainThread mainThread,
                                    AppCenterService appCenterService,
                                    ContactsListViewModel contactsListViewModel) : base(contactsListViewModel, appCenterService)
        {
            _mainThread = mainThread;

            var addContactButton = new ToolbarItem
            {
                Text = "+",
                AutomationId = AutomationIdConstants.AddContactButon
            };
            addContactButton.Clicked += HandleAddContactButtonClicked;
            ToolbarItems.Add(addContactButton);

            var refreshView = new RefreshView
            {
                RefreshColor = Color.Black,
                Content = new CollectionView
                {
                    ItemTemplate = new ContactsListDataTemplateSelector(),
                    BackgroundColor = Color.Transparent,
                    AutomationId = AutomationIdConstants.ContactsListView,
                }.Bind(CollectionView.ItemsSourceProperty, nameof(ContactsListViewModel.AllContactsList))
                 .Invoke(collectionView => collectionView.SelectionChanged += HandleSelectionChanged)

            }.Bind(RefreshView.CommandProperty, nameof(ContactsListViewModel.RefreshCommand))
             .Bind(RefreshView.IsRefreshingProperty, nameof(ContactsListViewModel.IsRefreshing));

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

            relativeLayout.Children.Add(refreshView,
                                        Constraint.Constant(0),
                                        Constraint.Constant(0),
                                        Constraint.RelativeToParent(parent => parent.Width),
                                        Constraint.RelativeToParent(parent => parent.Height));
            relativeLayout.Children.Add(restoreDeletedContactsButton,
                                        Constraint.RelativeToParent(parent => parent.Width / 2 - getWidth(parent, restoreDeletedContactsButton) / 2),
                                        Constraint.RelativeToParent(parent => parent.Height - getHeight(parent, restoreDeletedContactsButton) - 10));

            Content = relativeLayout;

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            static double getHeight(RelativeLayout parent, View view) => view.Measure(parent.Width, parent.Height).Request.Height;
            static double getWidth(RelativeLayout parent, View view) => view.Measure(parent.Width, parent.Height).Request.Width;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            AppCenterService.Track(AppCenterConstants.ContactsListPageAppeared);

            if (Content is Layout<View> layout
                && layout.Children.OfType<RefreshView>().First() is RefreshView refreshView)
            {
                refreshView.IsRefreshing = true;
            }
        }

        async void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = (CollectionView)sender;
            collectionView.SelectedItem = null;

            if (e.CurrentSelection.FirstOrDefault() is ContactModel selectedContactModel)
            {

                var contactDetailPage = ServiceCollection.Container.Resolve<ContactDetailPage>(new TypedParameter(typeof(bool), false), new TypedParameter(typeof(ContactModel), selectedContactModel));
                await Navigation.PushAsync(contactDetailPage);
            }
        }

        async void HandleAddContactButtonClicked(object sender, EventArgs e)
        {
            AppCenterService.Track(AppCenterConstants.AddContactButtonTapped);
            var contactDetailPage = ServiceCollection.Container.Resolve<ContactDetailPage>(new TypedParameter(typeof(bool), true), new TypedParameter(typeof(ContactModel), new ContactModel()));

            await _mainThread.InvokeOnMainThreadAsync(() => Navigation.PushModalAsync(new BaseNavigationPage(contactDetailPage)));
        }

        async void HandleRestoreDeletedContactsButtonClicked(object sender, EventArgs e)
        {
            var isDisplayAlertDialogConfirmed = await _mainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Restore Deleted Contacts", "Would you like to restore deleted contacts?", AlertDialogConstants.Yes, AlertDialogConstants.Cancel)).ConfigureAwait(false);

            if (isDisplayAlertDialogConfirmed)
                ViewModel.RestoreDeletedContactsCommand.Execute(null);
        }
    }
}
