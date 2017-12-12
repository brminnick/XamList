using System.Threading.Tasks;

using NUnit.Framework;

using XamList.Shared;

namespace XamList.UnitTests
{
    [TestFixture]
    public class ContactDetailViewModelTests : BaseUnitTest
    {
        [Test]
        public void ContactTest()
        {
            //Arrange
            var contactDetailViewModel = new ContactDetailViewModel();

            //Act
            contactDetailViewModel.Contact = new ContactModel
            {
                FirstName = TestConstants.TestFirstName,
                LastName = TestConstants.TestLastName,
                PhoneNumber = TestConstants.TestPhoneNumber
            };

            //Assert
            Assert.AreEqual(TestConstants.TestFirstName, contactDetailViewModel.FirstNameText);
            Assert.AreEqual(TestConstants.TestLastName, contactDetailViewModel.LastNameText);
            Assert.AreEqual(TestConstants.TestPhoneNumber, contactDetailViewModel.PhoneNumberText);
        }
    }
}
