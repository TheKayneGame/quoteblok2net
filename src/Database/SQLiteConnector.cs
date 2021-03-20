using SQLite;

namespace quoteblok2net.database
{
    public class SQLiteConnector
    {
        private static SQLiteConnection _db;

        public static SQLiteConnection GetInstance() {
            if (_db == null){
                _db = new SQLiteConnection("./Databases/Rooivalk.db");
            }
            return _db;
        }
    }
}