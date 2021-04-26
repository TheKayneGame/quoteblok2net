using System;
using MongoDB.Bson;
using MongoDB.Driver;
using quoteblok2net.BotSystem.Configuration;

namespace quoteblok2net.database
{
    public class MongoConnector
    {
        private static MongoClient _dbClient;
        private static IMongoDatabase _db;
        public static IMongoDatabase GetDatabaseInstance() {

            if (_db == null){
                
                String connectionString = $"mongodb://{ConfigManager.config.mongoDB.login}:{ConfigManager.config.mongoDB.pass}@{ConfigManager.config.mongoDB.address}";               
                _dbClient = new MongoClient(connectionString);
                _db = _dbClient.GetDatabase(ConfigManager.config.mongoDB.database);
            }
            return _db;
        }
    }
}