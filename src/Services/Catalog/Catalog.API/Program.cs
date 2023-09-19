using Catalog.API.Data;
using Catalog.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICatalogContext, CatalogContext>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (true/*app.Environment.IsDevelopment()*/)
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
