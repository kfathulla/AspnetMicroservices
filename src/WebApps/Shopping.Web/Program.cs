using AspnetRunBasics.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]));
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]));
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]));

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
