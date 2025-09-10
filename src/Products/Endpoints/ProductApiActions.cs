using Microsoft.EntityFrameworkCore;
using DataEntities;
using Products.Models;
using SearchEntities;
using Microsoft.AspNetCore.Http;
using Products.Models;
using Microsoft.Extensions.AI;
using System.Text;

namespace Products.Endpoints;

public static class ProductApiActions
{
    public static async Task<IResult> GetAllProducts(Products.Models.Context db)
    {
        var products = await db.Product.ToListAsync();
        return Results.Ok(products);
    }

    public static async Task<IResult> GetProductById(int id, Products.Models.Context db)
    {
        var model = await db.Product.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return model is not null ? Results.Ok(model) : Results.NotFound();
    }

    public static async Task<IResult> UpdateProduct(int id, Product product, Products.Models.Context db)
    {
        var existing = await db.Product.FirstOrDefaultAsync(m => m.Id == id);
        if (existing == null)
            return Results.NotFound();
        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.ImageUrl = product.ImageUrl;
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    public static async Task<IResult> CreateProduct(Product product, Products.Models.Context db)
    {
        db.Product.Add(product);
        await db.SaveChangesAsync();
        return Results.Created($"/api/Product/{product.Id}", product);
    }

    public static async Task<IResult> DeleteProduct(int id, Products.Models.Context db)
    {
        var affected = await db.Product
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();
        return affected == 1 ? Results.Ok() : Results.NotFound();
    }

    public static async Task<IResult> SearchAllProducts(string search, Products.Models.Context db, IChatClient? chatClient = null)
    {
        List<Product> products = await db.Product
            .Where(p => EF.Functions.Like(p.Name, $"%{search}%"))
            .ToListAsync();

        var response = new ProductSearchResponse();
        response.Products = products;

        // Generate tailored LLM response
        if (chatClient != null && products.Count > 0)
        {
            try
            {
                var productDetails = new StringBuilder();
                for (int i = 0; i < products.Count; i++)
                {
                    var product = products[i];
                    productDetails.AppendLine($"- Product {i + 1}:");
                    productDetails.AppendLine($"  - Name: {product.Name}");
                    productDetails.AppendLine($"  - Description: {product.Description}");
                    productDetails.AppendLine($"  - Price: {product.Price}");
                }

                var systemPrompt = "You are a helpful assistant specializing in outdoor camping products. Generate a friendly and informative response about the products found.";
                var userPrompt = $"User searched for: '{search}'\n\nFound {products.Count} products:\n{productDetails}\n\nGenerate a helpful response that summarizes the search results and highlights key product features.";

                var messages = new List<ChatMessage>
                {
                    new(ChatRole.System, systemPrompt),
                    new(ChatRole.User, userPrompt)
                };

                var chatResponse = await chatClient.GetResponseAsync(messages);
                response.Response = chatResponse.Text ?? $"{products.Count} products found for '{search}'";
            }
            catch (Exception ex)
            {
                // Fallback to simple response if LLM fails
                response.Response = products.Count > 0 ?
                    $"Found {products.Count} products matching '{search}'. Check out the options below!" :
                    $"No products found for '{search}'. Try searching with different keywords.";
            }
        }
        else
        {
            // Fallback response when no chat client or no products found
            response.Response = products.Count > 0 ?
                $"Found {products.Count} products matching '{search}'. Check out the options below!" :
                $"No products found for '{search}'. Try searching with different keywords.";
        }

        return Results.Ok(response);
    }
}
