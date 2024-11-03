using AsyncCommunication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<OrderHandler>();
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("Caching");
    options.SchemaName = "dbo";
    options.TableName = "TestCache";
});

WebApplication app = builder.Build();
app.MapGet("/order/{orderId:int}", async (
    [FromRoute] int orderId,
    [FromServices] OrderHandler orderHandler) =>
{
    await orderHandler.HandleOrderAsync(orderId);
    return Results.Accepted(value: "Order is processing...");
});

app.MapGet("/status/{orderId:int}", async (
    [FromRoute] int orderId,
    [FromServices] IDistributedCache distributedCache) =>
{
    string? orderStatus = await distributedCache.GetStringAsync($"Order-{orderId}");
    return !string.IsNullOrWhiteSpace(orderStatus) ? Results.Ok(orderStatus) : Results.NotFound("Order not found");
});

app.MapGet("/data/{orderId:int}", async (
    [FromRoute] int orderId,
    [FromServices] IDistributedCache distributedCache) =>
{
    string? orderData = await distributedCache.GetStringAsync($"OrderData-{orderId}");
    return !string.IsNullOrWhiteSpace(orderData) ? Results.Ok(orderData) : Results.NotFound("OrderData not found");
});

app.Run();