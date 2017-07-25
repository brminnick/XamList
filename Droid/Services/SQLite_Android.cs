using System.IO;

using SQLite;

using Xamarin.Forms;

using XamList.Droid;

[assembly: Dependency(typeof(SQLite_Android))]
namespace XamList.Droid
{
	public class SQLite_Android : ISQLite
	{
		#region ISQLite implementation
		public SQLiteAsyncConnection GetConnection()
		{
			var sqliteFilename = "XamList.db3";
			string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
			var path = Path.Combine(documentsPath, sqliteFilename);

			var conn = new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

			// Return the database connection 
			return conn;
		}
		#endregion
	}
}

