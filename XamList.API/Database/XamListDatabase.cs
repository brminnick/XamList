using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace XamList.API.Database
{
    public class XamListDatabase
    {
        #region Constant Fields
        readonly static string _connectionString = ConfigurationManager.ConnectionStrings["XamListDatabaseConnectionString"].ConnectionString;
        #endregion

        #region Methods
        public static async Task<List<ContactModel>> GetAllContactModels()
        {
            Func<DataContext, List<ContactModel>> getAllAzureAnswersFunction = x => x.GetTable<ContactModel>().ToList();
            return await PerformDatabaseFunction(getAllAzureAnswersFunction);
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