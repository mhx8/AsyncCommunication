using System.Net.Http.Json;

using HttpClient client = new();
HttpResponseMessage? response;

Console.WriteLine("Enter a order id: ");
string? orderId = Console.ReadLine();

if (int.TryParse(orderId, out int id))
{
    response = await client.GetAsync($"http://localhost:5172/order/{id}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}
else
{
    Console.WriteLine("Invalid order id.");
}

string? oderStatus = null;
while (oderStatus != "Finished")
{
    response = await client.GetAsync($"http://localhost:5172/status/{orderId}");
    oderStatus = await response.Content.ReadFromJsonAsync<string>();
    Console.WriteLine($"Order is not ready yet. Status: {oderStatus}. Please wait...");
    await Task.Delay(2000);
}

Console.WriteLine("Order is ready! Press any key to retrieve the order data.");
Console.ReadKey();

response = await client.GetAsync($"http://localhost:5172/data/{orderId}");
Console.WriteLine(await response.Content.ReadAsStringAsync());