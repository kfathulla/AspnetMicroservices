using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DeleteOrderCommandHandler> logger;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<DeleteOrderCommandHandler> logger)
        {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await this.orderRepository.GetByIdAsync(request.Id);
            if (orderToDelete == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }

            await this.orderRepository.DeleteAsync(orderToDelete);

            this.logger.LogInformation($"Order {orderToDelete.Id} is successfully deleted.");
        }
    }
}
