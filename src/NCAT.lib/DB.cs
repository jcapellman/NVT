using LiteDB;

using NCAT.lib.Objects;

namespace NCAT.lib
{
    public class DB
    {
        private const string DB_FILENAME = "ips.db";

        public static NetworkConnectionItem CheckDB(string ipAddress)
        {
            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var items = db.GetCollection<NetworkConnectionItem>();

                return items.FindOne(a => a.IPAddress == ipAddress);
            }
        }

        public static void AddToDB(NetworkConnectionItem item)
        {
            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var items = db.GetCollection<NetworkConnectionItem>();

                items.Insert(item);
            }
        }
    }
}