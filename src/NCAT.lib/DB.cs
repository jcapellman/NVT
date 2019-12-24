using System;

using LiteDB;

using NCAT.lib.Objects;

namespace NCAT.lib
{
    public class DB
    {
        private const string DB_FILENAME = "ips.db";

        public static NetworkConnectionItem CheckDB(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var items = db.GetCollection<NetworkConnectionItem>();

                return items.FindOne(a => a.IPAddress == ipAddress);
            }
        }

        public static void AddToDB(NetworkConnectionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var items = db.GetCollection<NetworkConnectionItem>();

                items.Insert(item);
            }
        }
    }
}