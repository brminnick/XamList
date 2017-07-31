using System;
using System.Linq;
using System.Data.Linq;
using System.Diagnostics;
using System.Configuration;
using System.Web.Http.OData;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;

namespace XamList.Backend.Common
{
    public class XamListDatabase
    {
        #region Constant Fields
        readonly static string _connectionString = ConfigurationManager.ConnectionStrings["XamListDatabaseConnectionString"].ConnectionString;
        #endregion

        #region Methods
        public static async Task<IList<ContactModel>> GetAllContactModels()
        {
            Func<DataContext, IList<ContactModel>> getAllContactModelsFunction = dataContext => dataContext.GetTable<ContactModel>().ToList();
            return await PerformDatabaseFunction(getAllContactModelsFunction);
        }

        public static async Task<ContactModel> GetContactModel(string id)
        {
            Func<DataContext, ContactModel> getContactModelFunction = dataContext => dataContext.GetTable<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();
            return await PerformDatabaseFunction(getContactModelFunction);
        }

        public static async Task<ContactModel> InsertContactModel(ContactModel contact)
        {
            Func<DataContext, ContactModel> insertContactModelFunction = dataContext =>
            {
                if (string.IsNullOrWhiteSpace(contact.Id))
                    contact.Id = Guid.NewGuid().ToString();

                contact.CreatedAt = DateTimeOffset.UtcNow;
                contact.UpdatedAt = DateTimeOffset.UtcNow;

                dataContext.GetTable<ContactModel>().InsertOnSubmit(contact);

                return contact;
            };

            return await PerformDatabaseFunction(insertContactModelFunction);
        }

        public static async Task<ContactModel> PatchContactModel(ContactModel contact)
        {
            var contactModelDelta = new Delta<ContactModel>();

            contactModelDelta.TrySetPropertyValue(nameof(ContactModel.CreatedAt), contact.CreatedAt);
            contactModelDelta.TrySetPropertyValue(nameof(ContactModel.FirstName), contact.FirstName);
            contactModelDelta.TrySetPropertyValue(nameof(ContactModel.IsDeleted), contact.IsDeleted);
            contactModelDelta.TrySetPropertyValue(nameof(ContactModel.LastName), contact.LastName);
            contactModelDelta.TrySetPropertyValue(nameof(ContactModel.PhoneNumber), contact.PhoneNumber);
            contactModelDelta.TrySetPropertyValue(nameof(ContactModel.UpdatedAt), DateTimeOffset.UtcNow);

            return await PatchContactModel(contact.Id, contactModelDelta);
        }

        public static async Task<ContactModel> PatchContactModel(string id, Delta<ContactModel> contact)
        {
            Func<DataContext, ContactModel> patchContactModelFunction = dataContext =>
            {
                var contactFromDatabase = dataContext.GetTable<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                contact.Patch(contactFromDatabase);
                contactFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return contactFromDatabase;
            };

            return await PerformDatabaseFunction(patchContactModelFunction);
        }

        public static async Task<ContactModel> DeleteContactModel(string id)
        {
            Func<DataContext, ContactModel> deleteContactModelFunction = dataContext =>
            {
                var contactFromDatabase = dataContext.GetTable<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                contactFromDatabase.IsDeleted = true;
                contactFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return contactFromDatabase;
            };

            return await PerformDatabaseFunction(deleteContactModelFunction);
        }

        static async Task<TResult> PerformDatabaseFunction<TResult>(Func<DataContext, TResult> databaseFunction) where TResult : class
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var dbContext = new DataContext(connection);

                var signUpTransaction = connection.BeginTransaction();
                dbContext.Transaction = signUpTransaction;

                try
                {
                    return databaseFunction?.Invoke(dbContext) ?? default(TResult);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.ToString());
                    Debug.WriteLine("");

                    return default(TResult);
                }
                finally
                {
                    dbContext.SubmitChanges();
                    signUpTransaction.Commit();
                    connection.Close();
                }
            }
        }
        #endregion
    }
}