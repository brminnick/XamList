using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactsListTextCell : TextCell
    {
        public ContactsListTextCell()
        {
            TextColor = ColorConstants.TextColor;
            DetailColor = ColorConstants.DetailColor;

            var deleteMenuItem = new MenuItem
            {
                Text = "Delete",
                IsDestructive = true
            };
            deleteMenuItem.Clicked += HandleDeleteClicked;

            ContextActions.Add(deleteMenuItem);
        }

        ContactsListViewModel ContactsListViewModel => (ContactsListViewModel)ContactsListPage.BindingContext;

        ContactsListPage ContactsListPage
        {
            get
            {
                var navigationPage = (NavigationPage)Application.Current.MainPage;
                return (ContactsListPage)navigationPage.Navigation.NavigationStack.First();
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Text = string.Empty;
            Detail = string.Empty;

            var item = (ContactModel)BindingContext;

            Text = item.FullName;
            Detail = item.PhoneNumber;
        }

        async void HandleDeleteClicked(object sender, EventArgs e)
        {
            var contactSelected = (ContactModel)BindingContext;

            await Task.WhenAll(ApiService.DeleteContactModel(contactSelected.Id),
                               ContactDatabase.DeleteContact(contactSelected)).ConfigureAwait(false);

            ContactsListViewModel.RefreshCommand.Execute(null);
        }
    }
}
