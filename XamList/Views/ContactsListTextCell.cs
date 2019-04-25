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
		ContactsListViewModel ContactsListViewModel => ContactsListPage.BindingContext as ContactsListViewModel;

		ContactsListPage ContactsListPage
		{
			get
			{
				var navigationPage = Application.Current.MainPage as NavigationPage;
				return navigationPage.Navigation.NavigationStack.FirstOrDefault() as ContactsListPage;
			}
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

			await Task.WhenAll(ApiService.DeleteContactModel(contactSelected?.Id),
							   ContactDatabase.DeleteContact(contactSelected)).ConfigureAwait(false);

			ContactsListViewModel.RefreshCommand?.Execute(null);
		}
		#endregion
	}
}
