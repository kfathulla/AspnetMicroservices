using AutoMapper;
using Discount.Grpc.Extensions;
using Discount.Grpc.Mapper;
using Discount.Grpc.Repositories;
using Discount.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddTransient<IDiscountRepository, DiscountRepository>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<DiscountService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Services.MigrateDatabase<Program>();

app.Run();
