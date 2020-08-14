using System;
using System.Linq;
using Autofac;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using XamList.Mobile.Shared;
using XamList.Shared;
using static XamList.MarkupExtensions;
using static Xamarin.Forms.Markup.GridRowsColumns;

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

            ToolbarItems.Add(new ToolbarItem { Text = "+", AutomationId = AutomationIdConstants.AddContactButon }
                                .Invoke(toolBarItem => toolBarItem.Clicked += HandleAddContactButtonClicked));

            Title = PageTitleConstants.ContactsListPage;

            Content = new Grid
            {
                RowDefinitions = Rows.Define(
                    (Row.List, Star),
                    (Row.RestoreButton, AbsoluteGridLength(50))),

                Children =
                {
                    new RefreshView
                    {
                        RefreshColor = Color.Black,
                        Content = new CollectionView
                        {
                            ItemTemplate = new ContactsListDataTemplateSelector(),
                            BackgroundColor = Color.Transparent,
                            AutomationId = AutomationIdConstants.ContactsListView,
                            SelectionMode = SelectionMode.Single
                        }.Bind(CollectionView.ItemsSourceProperty, nameof(ContactsListViewModel.AllContactsList))
                         .Invoke(collectionView => collectionView.SelectionChanged += HandleSelectionChanged)

                    }.RowSpan(All<Row>())
                     .Bind(RefreshView.CommandProperty, nameof(ContactsListViewModel.RefreshCommand))
                     .Bind(RefreshView.IsRefreshingProperty, nameof(ContactsListViewModel.IsRefreshing)),

                    new Button
                    {
                        Text = "Restore Deleted Contacts",
                        TextColor = ColorConstants.TextColor,
                        AutomationId = AutomationIdConstants.RestoreDeletedContactsButton,
                        BackgroundColor = new Color(ColorConstants.NavigationBarBackgroundColor.R, ColorConstants.NavigationBarBackgroundColor.G, ColorConstants.NavigationBarBackgroundColor.B, 0.25)
                    }.Padding(10, 5).Margin(25).Center()
                     .Row(Row.RestoreButton)
                     .Invoke(button => button.Clicked += HandleRestoreDeletedContactsButtonClicked)
                }
            };

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        enum Row { List, RestoreButton }

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
