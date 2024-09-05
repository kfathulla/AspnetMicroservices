using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = this.mapper.Map<Order>(request);
            var newOrder = await this.orderRepository.AddAsync(orderEntity);

            this.logger.LogInformation($"Order {newOrder.Id} is successfully created.");

            await SendMail(newOrder);

            return newOrder.Id;
        }

        private async Task SendMail(Order order)
        {
            var email = new Email() { To = "kfathulla03@gmail.com", Body = $"Order was created.", Subject = "Order was created" };

            try
            {
                await this.emailService.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Order {order.Id} failed due to an error with the mail service: {ex.Message}");
            }
        }
    }
}
