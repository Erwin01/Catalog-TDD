using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogAPI.Entities
{
    public class Item
    {

        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
