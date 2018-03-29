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
            var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

            await databaseConnection.InsertOrReplaceAsync(contact).ConfigureAwait(false);
        }

        public static async Task PatchContactModel(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

			await databaseConnection.UpdateAsync(contact).ConfigureAwait(false);
        }


        public static async Task<int> GetContactCount()
        {
            var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

			return await databaseConnection.Table<ContactModel>().CountAsync().ConfigureAwait(false);
        }

        public static async Task<List<ContactModel>> GetAllContacts()
        {
            var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

			return await databaseConnection.Table<ContactModel>().ToListAsync().ConfigureAwait(false);
        }

        public static async Task<ContactModel> GetContact(string id)
        {
            var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

			return await databaseConnection.Table<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public static async Task DeleteContact(ContactModel contact)
        {
			var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

            contact.IsDeleted = true;

			await databaseConnection.UpdateAsync(contact).ConfigureAwait(false);
        }

#if DEBUG
        public static async Task RemoveContact(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnectionAsync().ConfigureAwait(false);

			await databaseConnection.DeleteAsync(contact).ConfigureAwait(false);
        }
#endif  
        #endregion
    }
}
