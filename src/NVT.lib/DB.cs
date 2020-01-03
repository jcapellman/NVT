using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using NVT.lib.JSONObjects;
using NVT.lib.Objects;

namespace NVT.lib
{
    public class DB
    {
        private const string DB_FILENAME = "ips.db";

        public static bool CheckDB(ref NetworkConnectionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var items = db.GetCollection<NetworkConnectionItem>();

                var ipAddress = item.IPAddress;

                var existingItem = items.FindOne(a => a.IPAddress == ipAddress);

                if (existingItem == null)
                {
                    return false;
                }

                item.City = existingItem.City;
                item.Country = existingItem.Country;
                item.ISP = existingItem.ISP;
                item.Latitude = existingItem.Latitude;
                item.Longitude = existingItem.Longitude;

                return true;
            }
        }

        public static List<IPAPIJsonObject> CheckDBForIPs(string[] ipAddresses)
        {
            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var items = db.GetCollection<IPAPIJsonObject>();

                return items.Find(a => ipAddresses.Contains(a.query)).ToList();
            }
        }

        public static void AddToDB(List<IPAPIJsonObject> items)
        {
            using (var db = new LiteDatabase(DB_FILENAME))
            {
                var dbItems = db.GetCollection<IPAPIJsonObject>();

                foreach (var item in items)
                {
                    item.Id = default;
                }

                dbItems.InsertBulk(items);
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