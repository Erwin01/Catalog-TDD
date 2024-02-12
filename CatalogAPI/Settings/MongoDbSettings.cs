using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Settings
{
    public class MongoDbSettings
    {

        public MongoClient mongoClient;
        public IMongoDatabase mongoDatabase;

        public MongoDbSettings()
        {
            mongoClient = new MongoClient("mongodb://127.0.0.1:27017");

            mongoDatabase = mongoClient.GetDatabase("Catalog");
        }


        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }



        public string ConnectionString
        {
            get
            {
                return $"mongodb://{Host}:{Port}";
                //return $"mongodb://{User}:{Password}@{Host}:{Port}";
            }
        }

    }
}
