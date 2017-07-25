using SQLite;

namespace XamList
{
	public interface ISQLite
	{
		SQLiteAsyncConnection GetConnection();
	}
}

