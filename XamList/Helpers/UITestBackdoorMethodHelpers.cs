#if DEBUG
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Mobile.Common;

namespace XamList
{
    public static class UITestBackdoorMethodHelpers
    {
        public static async Task RemoveTestContactsFromLocalDatabase()
        {
            var contactsList = await ContactDatabase.GetAllContacts();

            var testContactsList = contactsList.Where(x =>
                                                    x.FirstName.Equals(TestConstants.TestFirstName) &&
                                                    x.LastName.Equals(TestConstants.TestLastName) &&
                                                    x.PhoneNumber.Equals(TestConstants.TestPhoneNumber)).ToList();

            var removedContactTaskList = new List<Task>();
            foreach (var contact in testContactsList)
                removedContactTaskList.Add(ContactDatabase.RemoveContact(contact));

            await Task.WhenAll(removedContactTaskList);
        }
    }
}
#endif
