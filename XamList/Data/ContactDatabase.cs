using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;

namespace XamList
{
    public abstract class ContactDatabase : BaseDatabase
    {
        #region Methods
        public static async Task SaveContact(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnectionAsync();

            await databaseConnection.InsertOrReplaceAsync(contact);
        }

        public static async Task<int> GetContactCount()
        {
            var databaseConnection = await GetDatabaseConnectionAsync();

            return await databaseConnection.Table<ContactModel>().CountAsync();
        }

        public static async Task<List<ContactModel>> GetAllContacts()
        {
			var databaseConnection = await GetDatabaseConnectionAsync();

			return await databaseConnection.Table<ContactModel>().ToListAsync();
        }

        public static async Task<ContactModel> GetContact(string id)
        {
			var databaseConnection = await GetDatabaseConnectionAsync();

            return await databaseConnection.Table<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public static async Task DeleteContact(ContactModel contact)
        {
			var databaseConnection = await GetDatabaseConnectionAsync();

            contact.IsDeleted = true;

            await databaseConnection.UpdateAsync(contact);
		}
        #endregion
    }
}
