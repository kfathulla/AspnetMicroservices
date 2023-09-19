using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public CatalogContext(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = mongoClient.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Products = database.GetCollection<Product>(nameof(Products));
            CatalogContextSeed.SeedData(Products);
        }

    }
}
