using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository basketRepository;
        private readonly DiscountGrpcService discountGrpcService;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            this.basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            this.discountGrpcService = discountGrpcService;
            this.publishEndpoint = publishEndpoint;
            this.mapper = mapper;
        }

        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var basket = await this.basketRepository.GetBasketAsync(username);

            return Ok(basket ?? new ShoppingCart(username));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {
            foreach (var item in shoppingCart.Items)
            {
                var coupon = await this.discountGrpcService.GetDiscountAsync(item.ProductName);
                item.Price -= coupon.Amount;
            }

            var basket = await this.basketRepository.UpdateBasketAsync(shoppingCart);

            return Ok(basket);
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            bool success = await this.basketRepository.DeleteBasketAsync(username);

            return Ok(success);
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // get existing basket with total price 
            // Create basketCheckoutEvent -- Set TotalPrice on basketCheckout eventMessage
            // send checkout event to rabbitmq
            // remove the basket

            // get existing basket with total price
            var basket = await this.basketRepository.GetBasketAsync(basketCheckout.Username);
            if (basket == null)
            {
                return BadRequest();
            }

            // send checkout event to rabbitmq
            var eventMessage = this.mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await this.publishEndpoint.Publish(eventMessage);

            // remove the basket
            await this.basketRepository.DeleteBasketAsync(basket.Username);

            return Accepted();
        }
    }
}
