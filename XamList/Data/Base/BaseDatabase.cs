using System.Threading.Tasks;

using SQLite;

using Xamarin.Forms;

using XamList.Shared;

namespace XamList
{
    public abstract class BaseDatabase
    {
        #region Constant Fields
        static readonly SQLiteAsyncConnection _databaseConnection = DependencyService.Get<ISQLite>().GetConnection();
        #endregion

        #region Fields
        static bool _isInitialized;
        #endregion

        #region Methods
        protected static async Task<SQLiteAsyncConnection> GetDatabaseConnectionAsync()
        {
            if (!_isInitialized)
				await Initialize();
                
            return _databaseConnection;
        }

        static async Task Initialize()
        {
            await _databaseConnection.CreateTableAsync<ContactModel>();
            _isInitialized = true;
        }
        #endregion

    }
}
