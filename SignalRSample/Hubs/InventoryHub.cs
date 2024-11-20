using Microsoft.AspNetCore.SignalR;
using SignalRSample.Models;
using System.Collections.Concurrent;

namespace SignalRSample.Hubs
{
    public class InventoryHub : Hub
    {
        // In-memory storage for demo purposes
        private static readonly ConcurrentDictionary<string, Product> Products = new ConcurrentDictionary<string, Product>
        {
            ["1"] = new Product { Id = "1", Name = "Product A", Quantity = 10 },
            ["2"] = new Product { Id = "2", Name = "Product B", Quantity = 20 },
            ["3"] = new Product { Id = "3", Name = "Product C", Quantity = 15 }
        };

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("ReceiveProducts", Products.Values);
            return base.OnConnectedAsync();
        }

        public async Task DecrementQuantity(string productId)
        {
            if (Products.TryGetValue(productId, out var product))
            {
                if (product.Quantity > 0)
                {
                    product.Quantity--;

                    // Notify all clients of the updated quantity
                    await Clients.All.SendAsync("QuantityUpdated", product.Id, product.Quantity);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", $"Product with Id {product.Id} is out of stock.");
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", $"Product with Id {productId} not found.");
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            return Products.Values;
        }
    }
}
