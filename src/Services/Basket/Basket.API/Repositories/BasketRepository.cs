using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache distributedCache;

        public BasketRepository(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<ShoppingCart> GetBasketAsync(string username)
        {
            var basket = await this.distributedCache.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart shoppingCart)
        {
            await this.distributedCache.SetStringAsync(shoppingCart.Username, JsonConvert.SerializeObject(shoppingCart));

            return await GetBasketAsync(shoppingCart.Username);
        }
        public async Task<bool> DeleteBasketAsync(string username)
        {
            await this.distributedCache.RemoveAsync(username);

            return true;
        }

    }
}
