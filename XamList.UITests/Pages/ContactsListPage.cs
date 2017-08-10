using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Xamarin.UITest;

using XamList.Shared;
using XamList.Mobile.Common;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace XamList.UITests
{
    public class ContactsListPage : BasePage
    {
        #region Constant Fields
        readonly Query _addContactButon;
        readonly Query _restoreContactsButton;
        readonly Query _contactsListView;
        #endregion

        #region Constructors
        public ContactsListPage(IApp app, Platform platform) : base(app, platform, PageTitles.ContactsListPage)
        {
            _addContactButon = x => x.Marked(AutomationIdConstants.AddContactButon);
            _restoreContactsButton = x => x.Marked(AutomationIdConstants.RestoreDeletedContactsButton);
            _contactsListView = x => x.Marked(AutomationIdConstants.ContactsListView);
        }
        #endregion

        #region Properties
        public bool IsRefreshActivityIndicatorDisplayed =>
            GetIsRefreshActivityIndicatorDisplayed();
        #endregion

        #region Methods
        public void TapRestoreDeletedContactsButton(bool shouldConfirmAlertDialog)
        {
            App.Tap(_restoreContactsButton);

            switch(shouldConfirmAlertDialog)
            {
                case true:
                    App.Tap(AlertDialogConstants.Yes);
                    break;
                case false:
                    App.Tap(AlertDialogConstants.Cancel);
                    break;
            }
        }

        public async Task DeleteContact(string firstName, string lastName, string phoneNumber)
        {
            var contact = new ContactModel
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };

			App.ScrollDownTo(contact.FullName);

			if (OniOS)
				App.SwipeRightToLeft(contact.FullName);
			else
				App.TouchAndHold(contact.FullName);

			App.Tap("Delete");

            await Task.Delay(1000);
        }

        public void TapAddContactButton()
        {
            switch (OniOS)
            {
                case true:
                    App.Tap(_addContactButon);
                    break;
                default:
                    App.Tap(x => x.Class("ActionMenuItemView"));
                    break;
            }

            App.Screenshot("Tapped Add Contact Button");
        }

        public bool DoesContactExist(string firstName, string lastName, string phoneNumber, int timeoutInSeconds = 10)
		{
			var contact = new ContactModel
			{
				FirstName = firstName,
				LastName = lastName,
				PhoneNumber = phoneNumber
			};

			try
			{
				App.ScrollDownTo(contact.FullName, timeout: TimeSpan.FromSeconds(timeoutInSeconds));
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

        public async Task WaitForNoPullToRefreshActivityIndicatorAsync(int timeoutInSeconds = 10)
        {
            int loopCount = 0;

            while (IsRefreshActivityIndicatorDisplayed)
            {
				if (loopCount / 10 > timeoutInSeconds)
					Assert.Fail("WaitForNoPullToRefreshActivityIndicatorAsync Failed");

                loopCount++;
                await Task.Delay(100);
            }
        }

        public async Task WaitForPullToRefreshActivityIndicatorAsync(int timeoutInSeconds = 10)
        {
            int loopCount = 0;

            while (!IsRefreshActivityIndicatorDisplayed)
            {
                if (loopCount / 10 > timeoutInSeconds)
                    Assert.Fail("WaitForPullToRefreshActivityIndicatorAsync Failed");

                loopCount++;
                await Task.Delay(100);
            }
        }

        public void PullToRefresh()
        {
			var listViewQuery = App.Query(_contactsListView).FirstOrDefault();

			var topYCoordinate = listViewQuery.Rect.Y;
			var bottomYCoordinate = listViewQuery.Rect.Y + listViewQuery.Rect.Height;
			var centerXCoordinate = listViewQuery.Rect.CenterX;

			App.DragCoordinates(centerXCoordinate, topYCoordinate + 20, centerXCoordinate, bottomYCoordinate - 20);

			App.Screenshot("Pull To Refresh Triggered");
        }

        bool GetIsRefreshActivityIndicatorDisplayed()
        {
            switch (OniOS)
            {
                case true:
                    return App.Query(x => x.Class("UIRefreshControl")).Any();

                default:
                    return (bool)App.Query(x => x.Class("SwipeRefreshLayout").Invoke("isRefreshing")).FirstOrDefault();
            }
        }
        #endregion
    }
}
