using Microsoft.EntityFrameworkCore;
using DataEntities;
using Products.Models;
using SearchEntities;
using Microsoft.AspNetCore.Http;

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
        existing.IsDefault = product.IsDefault;
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

    public static async Task<IResult> SearchAllProducts(string search, Products.Models.Context db)
    {
        List<Product> products = await db.Product
            .Where(p => EF.Functions.Like(p.Name, $"%{search}%"))
            .ToListAsync();

        var response = new SearchResponse();
        response.Products = products;
        response.Response = products.Count > 0 ?
            $"{products.Count} Products found for [{search}]" :
            $"No products found for [{search}]";
        return Results.Ok(response);
    }

    public static async Task<IResult> GetDefaultProduct(Products.Models.Context db)
    {
        var model = await db.Product.AsNoTracking().FirstOrDefaultAsync(m => m.IsDefault);
        return model is not null ? Results.Ok(model) : Results.NotFound();
    }

    public static async Task<IResult> GetProductsByLocation(int locationId, Products.Models.Context db)
    {
        var productsAtLocation = await db.ProductsByLocation
            .Where(pl => pl.LocationId == locationId)
            .Include(pl => pl.Product)
            .Select(pl => new 
            {
                Product = pl.Product,
                Quantity = pl.Quantity
            })
            .ToListAsync();

        if (!productsAtLocation.Any())
            return Results.NotFound($"No products found at location {locationId}");

        return Results.Ok(productsAtLocation);
    }

    public static async Task<IResult> GetProductLocations(int productId, Products.Models.Context db)
    {
        var locations = await db.ProductsByLocation
            .Where(pl => pl.ProductId == productId)
            .Include(pl => pl.Location)
            .Select(pl => new 
            {
                Location = pl.Location,
                Quantity = pl.Quantity
            })
            .ToListAsync();

        if (!locations.Any())
            return Results.NotFound($"No locations found for product {productId}");

        return Results.Ok(locations);
    }
}
