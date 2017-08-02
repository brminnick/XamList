using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

using XamList.Shared;
using XamList.Mobile.Common;

namespace XamList
{
    public class ContactsListTextCell : TextCell
    {
        #region Constant Fields
        readonly MenuItem _deleteAction;
        #endregion

        #region Constructors
        public ContactsListTextCell()
        {
            TextColor = ColorConstants.TextColor;
            DetailColor = ColorConstants.DetailColor;

            _deleteAction = new MenuItem
            {
                Text = "Delete",
                IsDestructive = true
            };
            _deleteAction.Clicked += HandleDeleteClicked;

            ContextActions.Add(_deleteAction);
        }
        #endregion

        #region Finalizers
        ~ContactsListTextCell()
        {
            ContextActions.Remove(_deleteAction);
            _deleteAction.Clicked -= HandleDeleteClicked;
        }
        #endregion

        #region Properties
        ContactsListPage ContactsListPage
        {
            get
            {
                var navigationPage = Application.Current.MainPage as NavigationPage;
                return navigationPage.Navigation.NavigationStack.FirstOrDefault() as ContactsListPage;

            }
        }

        ContactsListViewModel ContactsListViewModel
        {
            get => ContactsListPage.BindingContext as ContactsListViewModel;
        }

        #endregion

        #region Methods
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Text = string.Empty;
            Detail = string.Empty;

            var item = BindingContext as ContactModel;

            Text = item.FullName;
            Detail = item.PhoneNumber;
        }

        async void HandleDeleteClicked(object sender, EventArgs e)
        {
            var contactSelected = BindingContext as ContactModel;

#pragma warning disable CS4014 // Await omitted intentionally
            Task.Run(async () => await APIService.DeleteContactModel(contactSelected)).ConfigureAwait(false);
#pragma warning restore CS4014 // Await omitted intentionally
			await ContactDatabase.DeleteContact(contactSelected).ConfigureAwait(false);

            ContactsListViewModel.RefreshCommand?.Execute(null);
        }
        #endregion
    }
}
