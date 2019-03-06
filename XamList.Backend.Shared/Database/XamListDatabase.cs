using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NPoco;
using XamList.Shared;

namespace XamList.Backend.Shared
{
    public static class XamListDatabase
    {
        #region Constant Fields
        readonly static string _connectionString = Environment.GetEnvironmentVariable("XamListDatabaseConnectionString");
        #endregion

        #region Methods
        public static Task<List<ContactModel>> GetAllContactModels()
        {
            return PerformDatabaseFunction(getContactModelsFunction);

            Task<List<ContactModel>> getContactModelsFunction(Database dataContext) => dataContext.FetchAsync<ContactModel>();
        }

        public static ContactModel GetContactModel(string id)
        {
            return PerformDatabaseFunction(getContactModelFunction).GetAwaiter().GetResult();

            Task<ContactModel> getContactModelFunction(Database dataContext)
            {
                var contact = dataContext.Fetch<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();
                return Task.FromResult(contact);
            }
        }

        public static Task<ContactModel> InsertContactModel(ContactModel contact)
        {
            return PerformDatabaseFunction(insertContactModelFunction);

            async Task<ContactModel> insertContactModelFunction(Database dataContext)
            {
                contact.CreatedAt = DateTimeOffset.UtcNow;
                contact.UpdatedAt = DateTimeOffset.UtcNow;

                await dataContext.InsertAsync(contact);

                return contact;
            }
        }

        public static Task<ContactModel> PatchContactModel(ContactModel contactModel)
        {
            return PerformDatabaseFunction(patchContactModelFunction);

            async Task<ContactModel> patchContactModelFunction(Database dataContext)
            {
                var contactFromDatabase = dataContext.Fetch<ContactModel>().Where(y => y.Id.Equals(contactModel.Id)).FirstOrDefault();

                contactFromDatabase.FirstName = contactModel.FirstName;
                contactFromDatabase.LastName = contactModel.LastName;
                contactFromDatabase.PhoneNumber = contactModel.PhoneNumber;
                contactFromDatabase.IsDeleted = contactModel.IsDeleted;
                contactFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                await dataContext.UpdateAsync(contactFromDatabase).ConfigureAwait(false);

                return contactFromDatabase;
            }
        }

        public static Task<ContactModel> DeleteContactModel(string id)
        {
            return PerformDatabaseFunction(deleteContactModelFunction);

            Task<ContactModel> deleteContactModelFunction(Database dataContext)
            {
                var contactFromDatabase = dataContext.Fetch<ContactModel>().Where(y => y.Id.Equals(id)).FirstOrDefault();

                contactFromDatabase.IsDeleted = true;

                return PatchContactModel(contactFromDatabase);
            }
        }

        public static Task<ContactModel> RemoveContactModel(string id)
        {
            return PerformDatabaseFunction(removeContactDatabaseFunction);

            async Task<ContactModel> removeContactDatabaseFunction(Database dataContext)
            {
                var answerModelFromDatabase = dataContext.Fetch<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();
                if (answerModelFromDatabase is null)
                    return null;

                await dataContext.DeleteAsync(answerModelFromDatabase).ConfigureAwait(false);

                return answerModelFromDatabase;
            }
        }

        static async Task<TResult> PerformDatabaseFunction<TResult>(Func<Database, Task<TResult>> databaseFunction) where TResult : class
        {
            using (var connection = new Database(_connectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance))
            {
                try
                {
                    return await databaseFunction?.Invoke(connection) ?? default;
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
        }
        #endregion
    }
}