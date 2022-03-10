using SQLite;
using Stock_Manager.Droid.Data;
using Stock_Manager.Interfaces;
using System.IO;
// use this platform specific code for the interface
//Sintax: [assembly: Dependency(typeof(namespace.class))]
[assembly: Xamarin.Forms.Dependency(typeof(SQLite_Android))]
namespace Stock_Manager.Droid.Data
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }

        public SQLiteConnection GetConnection()
        {
            var sqliteFileName = "StockManagerDB.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFileName);
            var conn = new SQLite.SQLiteConnection(path);

            return conn;

            //throw new NotImplementedException();
        }


    }
}