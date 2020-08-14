using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using SQLite;
using Xamarin.Essentials.Interfaces;

namespace XamList
{
    public abstract class BaseDatabase<T>
    {
        readonly SQLiteAsyncConnection _databaseConnection;

        public BaseDatabase(IFileSystem fileSystem)
        {
            var databasePath = Path.Combine(fileSystem.AppDataDirectory, "PunModelSQLite.db3");

            _databaseConnection = new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        }

        protected virtual async Task Init()
        {
            await _databaseConnection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
            await _databaseConnection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
        }

        protected async Task<TReturn> ExecuteDatabaseFunction<TReturn>(Func<SQLiteAsyncConnection, Task<TReturn>> action, int numRetries = 12)
        {
            if (!_databaseConnection.TableMappings.Any(x => x.MappedType == typeof(T)))
                await Init().ConfigureAwait(false);

            return await Policy.Handle<Exception>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(() => action(_databaseConnection)).ConfigureAwait(false);

            static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        }
    }
}
