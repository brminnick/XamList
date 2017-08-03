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
            ContactsListPage.TapAddContactButton();

            App.WaitForElement(ContactDetailsPage.Title);

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, shouldUseReturnKey);

            switch (shouldUseReturnKey)
            {
                case false:
                    ContactDetailsPage.TapSaveButton();
                    break;
            }

            await ContactsListPage.WaitForPullToRefreshActivityIndicatorAsync();
            await ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync();

            Assert.IsTrue(ContactsListPage.DoesViewCellExist($"{firstName} {lastName}"));
            Assert.IsTrue(ContactsListPage.DoesViewCellExist(phoneNumber));
        }

        [TestCase(Constants.TestFirstName, Constants.TestLastName, Constants.TestPhoneNumber)]
        public void EnterContactInformationThenPressCancel(string firstName, string lastName, string phoneNumber)
        {
            ContactsListPage.TapAddContactButton();

            App.WaitForElement(ContactDetailsPage.Title);

            ContactDetailsPage.PopulateAllTextFields(firstName, lastName, phoneNumber, false);
            ContactDetailsPage.TapCancelButton();

            Assert.IsFalse(ContactsListPage.DoesViewCellExist($"{firstName} {lastName}"));
        }

        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();

            App.Screenshot("App Launched");
            App.WaitForElement(ContactsListPage.Title);
        }
        #endregion
    }
}
