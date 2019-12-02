using System.Threading.Tasks;
using System.Collections.Generic;
using XamList.Shared;

namespace XamList
{
    public abstract class ContactDatabase : BaseDatabase
    {
        public static async Task SaveContact(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            await AttemptAndRetry(() => databaseConnection.InsertOrReplaceAsync(contact)).ConfigureAwait(false);
        }

        public static async Task PatchContactModel(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            await AttemptAndRetry(() => databaseConnection.UpdateAsync(contact)).ConfigureAwait(false);
        }

        public static async Task<int> GetContactCount()
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            return await AttemptAndRetry(() => databaseConnection.Table<ContactModel>().CountAsync()).ConfigureAwait(false);
        }

        public static async Task<List<ContactModel>> GetAllContacts()
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            return await AttemptAndRetry(() => databaseConnection.Table<ContactModel>().ToListAsync()).ConfigureAwait(false);
        }

        public static async Task<ContactModel> GetContact(string id)
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            return await AttemptAndRetry(() => databaseConnection.Table<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync()).ConfigureAwait(false);
        }

        public static async Task DeleteContact(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            contact.IsDeleted = true;

            await AttemptAndRetry(() => databaseConnection.UpdateAsync(contact)).ConfigureAwait(false);
        }

#if DEBUG
        public static async Task RemoveContact(ContactModel contact)
        {
            var databaseConnection = await GetDatabaseConnection<ContactModel>().ConfigureAwait(false);

            await AttemptAndRetry(() => databaseConnection.DeleteAsync(contact)).ConfigureAwait(false);
        }
#endif  
    }
}
