using System.Threading.Tasks;
using System.Collections.Generic;
using XamList.Shared;
using Xamarin.Essentials.Interfaces;

namespace XamList
{
    public class ContactDatabase : BaseDatabase<ContactModel>
    {
        public ContactDatabase(IFileSystem fileSystem) : base(fileSystem)
        {

        }

        public Task SaveContact(ContactModel contact) => ExecuteDatabaseFunction(connection => connection.InsertOrReplaceAsync(contact));

        public Task PatchContactModel(ContactModel contact) => ExecuteDatabaseFunction(connection => connection.UpdateAsync(contact));

        public Task<int> GetContactCount() => ExecuteDatabaseFunction(connection => connection.Table<ContactModel>().CountAsync());

        public Task<List<ContactModel>> GetAllContacts() => ExecuteDatabaseFunction(connection => connection.Table<ContactModel>().ToListAsync());

        public Task<ContactModel> GetContact(string id) => ExecuteDatabaseFunction(connection => connection.Table<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync());

        public Task DeleteContact(ContactModel contact)
        {
            contact.IsDeleted = true;

            return ExecuteDatabaseFunction(connection => connection.UpdateAsync(contact));
        }

#if DEBUG
        public Task RemoveContact(ContactModel contact) => ExecuteDatabaseFunction(connection => connection.DeleteAsync(contact));
#endif  
    }
}
