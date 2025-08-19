using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly ILogger<InventoryController> _logger;
    private static readonly Dictionary<string, ToolRecommendation> _inventory = new()
    {
        { "PAINT-ROLLER-9IN", new ToolRecommendation { Name = "Paint Roller", Sku = "PAINT-ROLLER-9IN", IsAvailable = true, Price = 12.99m, Description = "9-inch paint roller for smooth walls" } },
        { "BRUSH-SET-3PC", new ToolRecommendation { Name = "Paint Brush Set", Sku = "BRUSH-SET-3PC", IsAvailable = true, Price = 24.99m, Description = "3-piece brush set for detail work" } },
        { "DROP-CLOTH-9X12", new ToolRecommendation { Name = "Drop Cloth", Sku = "DROP-CLOTH-9X12", IsAvailable = true, Price = 8.99m, Description = "Plastic drop cloth protection" } },
        { "SAW-CIRCULAR-7IN", new ToolRecommendation { Name = "Circular Saw", Sku = "SAW-CIRCULAR-7IN", IsAvailable = true, Price = 89.99m, Description = "7.25-inch circular saw for wood cutting" } },
        { "STAIN-WOOD-QT", new ToolRecommendation { Name = "Wood Stain", Sku = "STAIN-WOOD-QT", IsAvailable = false, Price = 15.99m, Description = "1-quart wood stain in natural color" } },
        { "SAFETY-GLASSES", new ToolRecommendation { Name = "Safety Glasses", Sku = "SAFETY-GLASSES", IsAvailable = true, Price = 5.99m, Description = "Safety glasses for eye protection" } },
        { "GLOVES-WORK-L", new ToolRecommendation { Name = "Work Gloves", Sku = "GLOVES-WORK-L", IsAvailable = true, Price = 7.99m, Description = "Heavy-duty work gloves" } },
        { "DRILL-CORDLESS", new ToolRecommendation { Name = "Cordless Drill", Sku = "DRILL-CORDLESS", IsAvailable = true, Price = 79.99m, Description = "18V cordless drill with battery" } },
        { "LEVEL-2FT", new ToolRecommendation { Name = "Level", Sku = "LEVEL-2FT", IsAvailable = true, Price = 19.99m, Description = "2-foot aluminum level" } },
        { "TILE-CUTTER", new ToolRecommendation { Name = "Tile Cutter", Sku = "TILE-CUTTER", IsAvailable = false, Price = 45.99m, Description = "Manual tile cutting tool" } }
    };

    public InventoryController(ILogger<InventoryController> logger)
    {
        _logger = logger;
    }

    [HttpPost("search")]
    public async Task<ActionResult<ToolRecommendation[]>> SearchInventoryAsync([FromBody] InventorySearchRequest request)
    {
        try
        {
            _logger.LogInformation("Searching inventory for {Count} SKUs", request.Skus.Length);

            // Simulate inventory lookup delay
            await Task.Delay(500);

            var results = new List<ToolRecommendation>();

            foreach (var sku in request.Skus)
            {
                if (_inventory.TryGetValue(sku, out var item))
                {
                    // Simulate some dynamic pricing and availability
                    var enrichedItem = new ToolRecommendation
                    {
                        Name = item.Name,
                        Sku = item.Sku,
                        IsAvailable = item.IsAvailable && Random.Shared.NextDouble() > 0.1, // 10% chance of being out of stock
                        Price = item.Price * (decimal)(0.9 + Random.Shared.NextDouble() * 0.2), // Price variation ±10%
                        Description = item.Description
                    };
                    
                    results.Add(enrichedItem);
                }
                else
                {
                    // Create a generic item for unknown SKUs
                    results.Add(new ToolRecommendation
                    {
                        Name = $"Tool for SKU {sku}",
                        Sku = sku,
                        IsAvailable = false,
                        Price = 29.99m,
                        Description = $"Product not found in current inventory"
                    });
                }
            }

            return Ok(results.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching inventory");
            return StatusCode(500, "An error occurred while searching inventory");
        }
    }

    [HttpGet("search/{sku}")]
    public ActionResult<ToolRecommendation> GetItem(string sku)
    {
        try
        {
            _logger.LogInformation("Getting inventory item for SKU: {Sku}", sku);

            if (_inventory.TryGetValue(sku, out var item))
            {
                return Ok(item);
            }

            return NotFound($"Item with SKU {sku} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inventory item for SKU: {Sku}", sku);
            return StatusCode(500, "An error occurred while retrieving the item");
        }
    }

    [HttpGet("available")]
    public ActionResult<ToolRecommendation[]> GetAvailableItems()
    {
        try
        {
            _logger.LogInformation("Getting all available inventory items");

            var availableItems = _inventory.Values.Where(item => item.IsAvailable).ToArray();
            return Ok(availableItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available inventory items");
            return StatusCode(500, "An error occurred while retrieving available items");
        }
    }

    [HttpPost("check-availability")]
    public async Task<ActionResult<Dictionary<string, bool>>> CheckAvailabilityAsync([FromBody] string[] skus)
    {
        try
        {
            _logger.LogInformation("Checking availability for {Count} SKUs", skus.Length);

            // Simulate availability check delay
            await Task.Delay(300);

            var availability = new Dictionary<string, bool>();

            foreach (var sku in skus)
            {
                if (_inventory.TryGetValue(sku, out var item))
                {
                    availability[sku] = item.IsAvailable;
                }
                else
                {
                    availability[sku] = false;
                }
            }

            return Ok(availability);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking availability");
            return StatusCode(500, "An error occurred while checking availability");
        }
    }
}
