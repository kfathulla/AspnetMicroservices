using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration configuration;
        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Coupon> GetDiscountAsync(string productName)
        {
            using var connection = new NpgsqlConnection(this.configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
                ("SELECT * FROM Coupon WHERE ProductName = @productName", new { productName = productName });

            if (coupon == null)
            {
                return new Coupon
                {
                    ProductName = "No Discount",
                    Amount = 0,
                    Description = "No Discount Description"
                };
            }

            return coupon;
        }

        public async Task<Coupon> CreateDiscountAsync(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(this.configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            coupon.Id = await connection.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@productName, @description, @amount)" +
                " RETURNING Id ",
                new { productName = coupon.ProductName, description = coupon.Description, amount = coupon.Amount });

            return coupon;
        }

        public async Task<Coupon> UpdateDiscountAsync(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(this.configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            coupon.Id = await connection.QueryFirstOrDefaultAsync
                    ("UPDATE Coupon SET ProductName=@productName, Description = @description, Amount = @amount WHERE Id = @id " +
                    " RETURNING Id ",
                            new { productName = coupon.ProductName, description = coupon.Description, amount = coupon.Amount, id = coupon.Id });

            return coupon;
        }

        public async Task<bool> DeleteDiscountAsync(string productName)
        {
            using var connection = new NpgsqlConnection(this.configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @productName",
                new { productName = productName });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
