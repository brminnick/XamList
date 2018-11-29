using System.Threading.Tasks;

using NUnit.Framework;

using Xamarin.UITest;

using XamList.Mobile.Shared;

namespace XamList.UITests
{
    public class Tests : BaseUITest
    {
        #region Constructors
        public Tests(Platform platform): base(platform)
        {
        }
        #endregion

        #region Methods
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

			await ContactsListPage.DeleteContact(firstName, lastName, phoneNumber).ConfigureAwait(false);
            Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));

            ContactsListPage.TapRestoreDeletedContactsButton(shouldConfirmAlertDialog);
			
            switch(shouldConfirmAlertDialog)
            {
                case true:
                    ContactsListPage.WaitForPageToLoad();
					await ContactsListPage.WaitForPullToRefreshActivityIndicatorAsync().ConfigureAwait(false);
					await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().ConfigureAwait(false);
                    ContactsListPage.PullToRefresh();
					await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().ConfigureAwait(false);
                    break;
            }

            //Assert
            switch (shouldConfirmAlertDialog)
            {
                case true:
                    Assert.IsTrue(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
                    break;
                case false:
                    Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
                    break;
            }
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
			await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().ConfigureAwait(false);

            //Assert
            Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
        }

        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();

            App.Screenshot("App Launched");
        }

        Task AddContact(string firstName, string lastName, string phoneNumber, bool shouldUseReturnKey)
        {
            ContactsListPage.TapAddContactButton();

            ContactDetailsPage.WaitForPageToLoad();

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, shouldUseReturnKey);
            ContactDetailsPage.TapSaveButton();

            ContactsListPage.WaitForPageToLoad();
            return ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync();
        }
        #endregion
    }
}
