using System;
using MongoDB.Bson;
using MongoDB.Driver;
using quoteblok2net.BotSystem.Configuration;

namespace quoteblok2net.database
{
    public class MongoConnector
    {
        private MongoClient _dbClient;
        public IMongoDatabase db;

        public MongoConnector()
        {
            String connectionString = $"mongodb://{ConfigManager.config.mongoDB.login}:{ConfigManager.config.mongoDB.pass}@{ConfigManager.config.mongoDB.address}";
            _dbClient = new MongoClient(connectionString);
            db = _dbClient.GetDatabase(ConfigManager.config.mongoDB.database);
        }
    }
}