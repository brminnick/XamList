using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;
using XamList.Mobile.Shared;

namespace XamList.UITests
{
    public class Tests : BaseUITest
    {
        public Tests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void AppLaunches()
        {

        }

        [TestCase(TestConstants.TestFirstName, TestConstants.TestLastName, TestConstants.TestPhoneNumber, true)]
        [TestCase(TestConstants.TestFirstName, TestConstants.TestLastName, TestConstants.TestPhoneNumber, false)]
        public async Task AddContactTest(string firstName, string lastName, string phoneNumber, bool shouldUseReturnKey)
        {
            //Arrange

            //Act
            await AddContact(firstName, lastName, phoneNumber, shouldUseReturnKey).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RestoreDeletedContactTest(bool shouldConfirmAlertDialog)
        {
            //Arrange
            var firstName = TestConstants.TestFirstName;
            var lastName = TestConstants.TestLastName;
            var phoneNumber = TestConstants.TestPhoneNumber;

            //Act
            await AddContact(firstName, lastName, phoneNumber, false).ConfigureAwait(false);

            ContactsListPage.DeleteContact(firstName, lastName, phoneNumber);

            try
            {
                await ContactsListPage.WaitForPullToRefreshActivityIndicator(3).ConfigureAwait(false);
            }
            catch
            {

            }
            await ContactsListPage.WaitForNoPullToRefreshActivityIndicator().ConfigureAwait(false);

            Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));

            ContactsListPage.TapRestoreDeletedContactsButton(shouldConfirmAlertDialog);

            if (shouldConfirmAlertDialog)
            {
                ContactsListPage.WaitForPageToLoad();

                try
                {
                    await ContactsListPage.WaitForPullToRefreshActivityIndicator(3).ConfigureAwait(false);
                }
                catch
                {

                }

                await ContactsListPage.WaitForNoPullToRefreshActivityIndicator().ConfigureAwait(false);
                ContactsListPage.PullToRefresh();
                await ContactsListPage.WaitForNoPullToRefreshActivityIndicator().ConfigureAwait(false);
            }

            //Assert
            if (shouldConfirmAlertDialog)
                Assert.IsTrue(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
            else
                Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
        }

        [TestCase(TestConstants.TestFirstName, TestConstants.TestLastName, TestConstants.TestPhoneNumber)]
        public async Task EnterContactInformationThenPressCancel(string firstName, string lastName, string phoneNumber)
        {
            //Arrange

            //Act
            ContactsListPage.TapAddContactButton();

            ContactDetailsPage.WaitForPageToLoad();

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, false);
            ContactDetailsPage.TapCancelButton();

            ContactsListPage.WaitForPageToLoad();
            await ContactsListPage.WaitForNoPullToRefreshActivityIndicator().ConfigureAwait(false);

            //Assert
            Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
        }

        async Task AddContact(string firstName, string lastName, string phoneNumber, bool shouldUseReturnKey)
        {
            ContactsListPage.TapAddContactButton();

            ContactDetailsPage.WaitForPageToLoad();

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, shouldUseReturnKey);
            ContactDetailsPage.TapSaveButton();

            ContactsListPage.WaitForPageToLoad();

            try
            {
                await ContactsListPage.WaitForPullToRefreshActivityIndicator(3).ConfigureAwait(false);
            }
            catch
            {

            }

            await ContactsListPage.WaitForNoPullToRefreshActivityIndicator().ConfigureAwait(false);
        }
    }
}
