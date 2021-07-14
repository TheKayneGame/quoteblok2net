using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

//using quoteblok2net.BotSystem.Configuration;


namespace quoteblok2net.database
{
    public class MongoConnector
    {
        const string Login = "kayne";
        const string Pass = "VJ735fe%2F";
        const string Address = "chinook.blok-2.nl:11000";
        const string Database = "blok_2_bot_test";

        private static MongoClient _dbClient;
        private static IMongoDatabase _db;
        
        public static IMongoDatabase GetDatabaseInstance() {

            if (_db == null){
                
                String connectionString = $"mongodb://{Login}:{Pass}@{Address}";               
                _dbClient = new MongoClient(connectionString);
                _db = _dbClient.GetDatabase(Database);
            }
            return _db;
        }
    }
}