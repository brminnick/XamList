using System.Threading.Tasks;

using NUnit.Framework;

using XamList.Shared;

namespace XamList.UnitTests
{
    [TestFixture]
    public class ContactDetailViewModelTests : BaseUnitTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task ExecuteSaveButtonTappedCommandTest(bool isNewContact)
        {
            //Arrange
            var testContactModel = new ContactModel
            {
                FirstName = TestConstants.TestFirstName,
                LastName = TestConstants.TestLastName,
                PhoneNumber = TestConstants.TestPhoneNumber
            };
            var contactDetailViewModel = new ContactDetailViewModel();

            //Act


            //Assert
        }
    }
}
