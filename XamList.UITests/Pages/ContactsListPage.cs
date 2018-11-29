using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Android;

using XamList.Shared;
using XamList.Mobile.Shared;

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
        public ContactsListPage(IApp app, Platform platform) : base(app, platform, PageTitleConstants.ContactsListPage)
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

            if (shouldConfirmAlertDialog)
                App.Tap(AlertDialogConstants.Yes);
            else
                App.Tap(AlertDialogConstants.Cancel);
        }

        public void ScrollToTopOfList()
        {
            var query = App.Query().FirstOrDefault();

            App.TapCoordinates(query.Rect.CenterX, query.Rect.Y);
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

            switch (App)
            {
                case iOSApp iOSApp:
                    iOSApp.SwipeRightToLeft(contact.FullName);
                    break;

                case AndroidApp androidApp:
                    androidApp.TouchAndHold(contact.FullName);
                    break;

                default:
                    throw new NotSupportedException();
            }

            App.Tap("Delete");

            await Task.Delay(1000).ConfigureAwait(false);
        }

        public void TapAddContactButton()
        {
            switch (App)
            {
                case iOSApp iOSApp:
                    iOSApp.Tap(_addContactButon);
                    break;
                case AndroidApp androidApp:
                    androidApp.Tap(x => x.Class("ActionMenuItemView"));
                    break;

                default:
                    throw new NotSupportedException();
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                ScrollToTopOfList();
            }

        }

        public async Task WaitForNoPullToRefreshActivityIndicatorAsync(int timeoutInSeconds = 10)
        {
            int loopCount = 0;

            while (IsRefreshActivityIndicatorDisplayed)
            {
                if (loopCount / 10 > timeoutInSeconds)
                    Assert.Fail("WaitForNoPullToRefreshActivityIndicatorAsync Failed");

                loopCount++;
                await Task.Delay(100).ConfigureAwait(false);
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
                await Task.Delay(100).ConfigureAwait(false);
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
            switch (App)
            {
                case iOSApp iOSApp:
                    return iOSApp.Query(x => x.Class("UIRefreshControl")).Any();

                case AndroidApp androidApp:
                    return (bool)androidApp.Query(x => x.Class("SwipeRefreshLayout").Invoke("isRefreshing")).FirstOrDefault();

                default:
                    throw new NotSupportedException();
            }
        }
        #endregion
    }
}
