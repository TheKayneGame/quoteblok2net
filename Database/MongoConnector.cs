using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using quoteblok2net.Utilities.Configs;

namespace quoteblok2net.database
{
    public class MongoConnector
    {
        private static MongoClient _dbClient;
        private static IMongoDatabase _db;
        public static IMongoDatabase GetDatabaseInstance() {

            if (_db == null){
                string username = Program.config.GetSection("database:username").Get<string>();
                string password = Program.config.GetSection("database:password").Get<string>();
                string address = Program.config.GetSection("database:address").Get<string>();
                string database = Program.config.GetSection("database:database").Get<string>();
                
                string connectionString = $"mongodb://{username}:{password}@{address}";
                //Console.WriteLine(connectionString);               
                _dbClient = new MongoClient(connectionString);
                _db = _dbClient.GetDatabase(database);
            }
            return _db;
        }
    }
}