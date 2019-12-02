using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;
using XamList.Mobile.Shared;
using XamList.Shared;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace XamList.UITests
{
    public class ContactsListPage : BasePage
    {
        readonly Query _addContactButon, _restoreContactsButton, _contactsListView;

        public ContactsListPage(IApp app) : base(app, PageTitleConstants.ContactsListPage)
        {
            _addContactButon = x => x.Marked(AutomationIdConstants.AddContactButon);
            _restoreContactsButton = x => x.Marked(AutomationIdConstants.RestoreDeletedContactsButton);
            _contactsListView = x => x.Marked(AutomationIdConstants.ContactsListView);
        }


        public bool IsRefreshActivityIndicatorDisplayed => GetIsRefreshActivityIndicatorDisplayed();

        public void TapRestoreDeletedContactsButton(bool shouldConfirmAlertDialog)
        {
            App.Tap(_restoreContactsButton);

            if (shouldConfirmAlertDialog)
                App.Tap(AlertDialogConstants.Yes);
            else
                App.Tap(AlertDialogConstants.Cancel);
        }

        public void DeleteContact(string firstName, string lastName, string phoneNumber)
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
            catch
            {
                return false;
            }
        }

        public async Task WaitForNoPullToRefreshActivityIndicator(int timeoutInSeconds = 10)
        {
            int loopCount = 0;

            while (IsRefreshActivityIndicatorDisplayed)
            {
                if (loopCount / 10 > timeoutInSeconds)
                    throw new Exception($"{nameof(WaitForNoPullToRefreshActivityIndicator)} Failed");

                loopCount++;
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        public async Task WaitForPullToRefreshActivityIndicator(int timeoutInSeconds = 10)
        {
            int loopCount = 0;

            while (!IsRefreshActivityIndicatorDisplayed)
            {
                if (loopCount / 10 > timeoutInSeconds)
                    throw new Exception($"{nameof(WaitForPullToRefreshActivityIndicator)} Failed");

                loopCount++;
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        public void PullToRefresh() => BackdoorMethodHelpers.TriggerPullToRefresh(App);

        bool GetIsRefreshActivityIndicatorDisplayed() => App switch
        {
            iOSApp iOSApp => iOSApp.Query(x => x.Class("UIRefreshControl")).Any(),
            AndroidApp androidApp => (bool)androidApp.Query(x => x.Class("ListViewRenderer_SwipeRefreshLayoutWithFixedNestedScrolling").Invoke("isRefreshing")).First(),
            _ => throw new NotSupportedException(),
        };
    }
}
