using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository discountRepository;
        private readonly ILogger<DiscountService> logger;
        private readonly IMapper mapper;

        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper)
        {
            this.discountRepository = discountRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = await this.discountRepository.GetDiscountAsync(request.ProductName);

            if (coupon == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));

            this.logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

            var couponModel = this.mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = this.mapper.Map<Coupon>(request.Coupon);

            await this.discountRepository.CreateDiscountAsync(coupon);
            this.logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

            var couponModel = this.mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = this.mapper.Map<Coupon>(request.Coupon);

            await this.discountRepository.UpdateDiscountAsync(coupon);
            this.logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

            var couponModel = this.mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await this.discountRepository.DeleteDiscountAsync(request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }
    }
}
