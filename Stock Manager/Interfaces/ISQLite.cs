using SQLite;

namespace Stock_Manager.Interfaces
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
