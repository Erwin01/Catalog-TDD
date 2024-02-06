using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }

        public string ConnectionString 
        {
            get 
            {
                return $"mongodb://{Host}:{Port}";
            }
        }

    }
}
