using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

using NLog;

using NVT.lib.JSONObjects;

using Logger = NLog.Logger;

namespace NVT.REST.DAL
{
    public class MongoDatabase
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IMongoDatabase _db;

        private const string CollectionName = "ips";

        public MongoDatabase()
        {
            try
            {
                var client = new MongoClient();

                _db = client.GetDatabase(CollectionName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Initializing Mongo");

                throw;
            }
        }

        public void AddToDB(List<IPAPIJsonObject> items)
        {
            try
            {
                if (!items.Any())
                {
                    return;
                }

                var collection = _db.GetCollection<IPAPIJsonObject>(CollectionName);
                
                collection.InsertMany(items);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when AddToDB: {ex}");
            }
        }

        public List<IPAPIJsonObject> CheckDBForIPs(string[] ipAddresses)
        {
            try
            {
                var collection = _db.GetCollection<IPAPIJsonObject>(CollectionName);

                var filter = Builders<IPAPIJsonObject>.Filter.AnyIn(nameof(IPAPIJsonObject.query), ipAddresses);

                return collection.FindSync(filter).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when AddToDB: {ex}");

                return new List<IPAPIJsonObject>();
            }
        }
    }
}