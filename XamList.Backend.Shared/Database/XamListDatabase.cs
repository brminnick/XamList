using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XamList.Shared;

namespace XamList.Backend.Shared
{
    public class XamListDatabase
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("XamListDatabaseConnectionString") ?? string.Empty;

        public List<ContactModel> GetAllContactModels(Func<ContactModel, bool> wherePredicate)
        {
            using var connection = new XamListDatabaseContext();

            return connection.Contacts?.Where(wherePredicate).ToList() ?? new List<ContactModel>();
        }

        public List<ContactModel> GetAllContactModels() => GetAllContactModels(x => true);

        public Task<ContactModel> GetContactModel(string id)
        {
            return PerformDatabaseFunction(getContactModelFunction);

            Task<ContactModel> getContactModelFunction(XamListDatabaseContext dataContext) => dataContext.Contacts.SingleAsync(x => x.Id.Equals(id));
        }

        public Task<ContactModel> InsertContactModel(ContactModel contact)
        {
            return PerformDatabaseFunction(insertContactModelFunction);

            async Task<ContactModel> insertContactModelFunction(XamListDatabaseContext dataContext)
            {
                if (string.IsNullOrWhiteSpace(contact.Id))
                    contact.Id = Guid.NewGuid().ToString();

                contact.CreatedAt = DateTimeOffset.UtcNow;
                contact.UpdatedAt = DateTimeOffset.UtcNow;

                await dataContext.AddAsync(contact).ConfigureAwait(false);

                return contact;
            }
        }

        public Task<ContactModel> PatchContactModel(ContactModel contactModel)
        {
            return PerformDatabaseFunction(patchContactModelFunction);

            async Task<ContactModel> patchContactModelFunction(XamListDatabaseContext dataContext)
            {
                var contactFromDatabase = await dataContext.Contacts.SingleAsync(y => y.Id.Equals(contactModel.Id)).ConfigureAwait(false);

                contactFromDatabase.FirstName = contactModel.FirstName;
                contactFromDatabase.LastName = contactModel.LastName;
                contactFromDatabase.PhoneNumber = contactModel.PhoneNumber;
                contactFromDatabase.IsDeleted = contactModel.IsDeleted;
                contactFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                dataContext.Update(contactFromDatabase);

                return contactFromDatabase;
            }
        }

        public Task<ContactModel> DeleteContactModel(string id)
        {
            return PerformDatabaseFunction(deleteContactModelFunction);

            async Task<ContactModel> deleteContactModelFunction(XamListDatabaseContext dataContext)
            {
                var contactFromDatabase = await dataContext.Contacts.SingleAsync(y => y.Id.Equals(id)).ConfigureAwait(false);

                contactFromDatabase.IsDeleted = true;

                return await PatchContactModel(contactFromDatabase).ConfigureAwait(false);
            }
        }

        public Task<ContactModel> RemoveContactModel(string id)
        {
            return PerformDatabaseFunction(removeContactDatabaseFunction);

            async Task<ContactModel> removeContactDatabaseFunction(XamListDatabaseContext dataContext)
            {
                var answerModelFromDatabase = await dataContext.Contacts.SingleAsync(x => x.Id.Equals(id)).ConfigureAwait(false);

                dataContext.Remove(answerModelFromDatabase);

                return answerModelFromDatabase;
            }
        }

        static async Task<TResult> PerformDatabaseFunction<TResult>(Func<XamListDatabaseContext, Task<TResult>> databaseFunction) where TResult : class
        {
            using var connection = new XamListDatabaseContext();

            try
            {
                var result = await databaseFunction.Invoke(connection).ConfigureAwait(false);
                await connection.SaveChangesAsync().ConfigureAwait(false);

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine("");
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.ToString());
                Debug.WriteLine("");

                throw;
            }
        }

        class XamListDatabaseContext : DbContext
        {
            public XamListDatabaseContext() => Database.EnsureCreated();

            public DbSet<ContactModel>? Contacts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}