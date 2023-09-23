using Discount.Grpc.Entities;

namespace Discount.Grpc.Repositories
{
    public interface IDiscountRepository
    {
        Task<Coupon> GetDiscountAsync(string productName);
        Task<Coupon> CreateDiscountAsync(Coupon coupon);
        Task<Coupon> UpdateDiscountAsync(Coupon coupon);
        Task<bool> DeleteDiscountAsync(string productName);
    }
}
