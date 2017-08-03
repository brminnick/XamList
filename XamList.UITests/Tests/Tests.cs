using System.Threading.Tasks;

using NUnit.Framework;

using Xamarin.UITest;

namespace XamList.UITests
{
    public class Tests : BaseTest
    {
        #region Constructors
        public Tests(Platform platform) : base(platform)
        {
        }
        #endregion

        #region Methods
        [Test]
        public void AppLaunches()
        {

        }

        [TestCase(Constants.TestFirstName, Constants.TestLastName, Constants.TestPhoneNumber, true)]
        [TestCase(Constants.TestFirstName, Constants.TestLastName, Constants.TestPhoneNumber, false)]
        public async Task AddContactTest(string firstName, string lastName, string phoneNumber, bool shouldUseReturnKey)
        {
            //Arrange

            //Act
            await AddContact(firstName, lastName, phoneNumber, shouldUseReturnKey);

            //Assert
            Assert.IsTrue(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RestoreDeletedContactTest(bool shouldConfirmAlertDialog)
        {
            //Arrange
            var firstName = Constants.TestFirstName;
            var lastName = Constants.TestLastName;
            var phoneNumber = Constants.TestPhoneNumber;


            //Act
            await AddContact(firstName, lastName, phoneNumber, false);

            await ContactsListPage.DeleteContact(firstName, lastName, phoneNumber);
            Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));

            ContactsListPage.TapRestoreDeletedContactsButton(shouldConfirmAlertDialog);
			
            await ContactsListPage.WaitForPullToRefreshActivityIndicatorAsync();
			await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync();

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

        [TestCase(Constants.TestFirstName, Constants.TestLastName, Constants.TestPhoneNumber)]
        public async Task EnterContactInformationThenPressCancel(string firstName, string lastName, string phoneNumber)
        {
            //Arrange

            //Act
            ContactsListPage.TapAddContactButton();

            App.WaitForElement(ContactDetailsPage.Title);

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, false);
            ContactDetailsPage.TapCancelButton();

			await ContactsListPage.WaitForPullToRefreshActivityIndicatorAsync();
			await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync();

            //Assert
            Assert.IsFalse(ContactsListPage.DoesContactExist(firstName, lastName, phoneNumber));
        }

        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();

            App.Screenshot("App Launched");
            App.WaitForElement(ContactsListPage.Title);
        }

        async Task AddContact(string firstName, string lastName, string phoneNumber, bool shouldUseReturnKey)
        {
            ContactsListPage.TapAddContactButton();

            App.WaitForElement(ContactDetailsPage.Title);

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, shouldUseReturnKey);
            ContactDetailsPage.TapSaveButton();

            await ContactsListPage.WaitForPullToRefreshActivityIndicatorAsync();
            await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync();
        }
        #endregion
    }
}
