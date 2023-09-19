using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext catalogContext;

        public ProductRepository(ICatalogContext catalogContext)
        {
            this.catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
        }

        public async Task CreateProductAsync(Product product)
        {
            await catalogContext.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var updateResult = await this.catalogContext.Products
                .ReplaceOneAsync(p => p.Id == product.Id, product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(product => product.Id, id);

            var deleteResult = await this.catalogContext.Products
                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await this.catalogContext
                                .Products
                                .Find(product => true)
                                .ToListAsync();
        }

        public async Task<Product> GetProductAsync(string id)
        {
            return await this.catalogContext
                                .Products
                                .Find(product => product.Id == id)
                                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategoryAsync(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(product => product.Category, categoryName);

            return await this.catalogContext
                                .Products
                                .Find(filter)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByNameAsync(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(product => product.Name, name);

            return await this.catalogContext
                                .Products
                                .Find(filter)
                                .ToListAsync();
        }
    }
}
