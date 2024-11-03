using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace AsyncCommunication;

public class OrderHandler(
    IDistributedCache distributedCache)
{
    public Task HandleOrderAsync(
        int orderId)
    {
        _ = HandleOrderInternalAsync(orderId);
        return Task.CompletedTask;
    }

    private async Task HandleOrderInternalAsync(
        int orderId)
    {
        await ProcessingOrderAsync(orderId, OrderStatus.Started);
        await ProcessingOrderAsync(orderId, OrderStatus.Processing);
        await ProcessingOrderAsync(orderId, OrderStatus.Finalizing);

        // Save Order-Processing result
        await distributedCache.SetStringAsync($"OrderData-{orderId}",
            JsonSerializer.Serialize(new OrderResult("Order is done")));
        
        await ProcessingOrderAsync(orderId, OrderStatus.Finished);
    }

    private async Task ProcessingOrderAsync(
        int orderId,
        OrderStatus status)
    {
        await UpdateStatusInCacheAsync(orderId, status);
        await Task.Delay(5000);
    }

    private async Task UpdateStatusInCacheAsync(
        int orderId,
        OrderStatus status)
        => await distributedCache.SetStringAsync($"Order-{orderId}", status.ToString());
}