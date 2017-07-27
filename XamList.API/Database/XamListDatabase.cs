using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using XamList.Shared;

namespace XamList.API.Database
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

        public static async Task InsertContactModel(ContactModel contact)
        {
            Func<DataContext, object> insertContactModelFunction = dataContext =>
            {
                if (string.IsNullOrWhiteSpace(contact.Id))
                    contact.Id = Guid.NewGuid().ToString();

                contact.CreatedAt = DateTimeOffset.UtcNow;
                contact.UpdatedAt = DateTimeOffset.UtcNow;

                dataContext.GetTable<ContactModel>().InsertOnSubmit(contact);

                return null;
            };

            await PerformDatabaseFunction(insertContactModelFunction);
        }

        public static async Task PatchContactModel(ContactModel contact)
        {
            Func<DataContext, object> patchContactModelFunction = dataContext =>
            {
                var contactFromDatabase = dataContext.GetTable<ContactModel>().Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();
                contactFromDatabase.FirstName = contact.FirstName;
                contactFromDatabase.IsDeleted = contact.IsDeleted;
                contactFromDatabase.LastName = contact.LastName;
                contactFromDatabase.PhoneNumber = contact.PhoneNumber;
                contactFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return null;
            };

            await PerformDatabaseFunction(patchContactModelFunction);
        }

        public static async Task DeleteContactModel(string id)
        {
            Func<DataContext, object> deleteContactModelFunction = dataContext =>
            {
                var contactFromDatabase = dataContext.GetTable<ContactModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();
                contactFromDatabase.IsDeleted = true;

                return null;
            };

            await PerformDatabaseFunction(deleteContactModelFunction);
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