using AssignmentApi.Data;
using AssignmentApi.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ProductTest
{
     private AppDbContext GetDbContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    databaseName: Guid.NewGuid().ToString())
                .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_Product_Should_Save_Product()
    {
        var db = GetDbContext();

         var product = new Product { Name = "Mobile", Price = 1000, Quantity = 10 };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        Assert.Equal(1, await db.Products.CountAsync());
    }

    [Fact]
    public async Task Get_Product_Should_Return_All_Products()
    {
        var db = GetDbContext();

        db.Products.Add(new Product { Name = "Mobile", Price = 20000, Quantity = 2 });
        db.Products.Add(new Product { Name = "Laptop", Price = 100000, Quantity = 1 });
        await db.SaveChangesAsync();

        var retrievedProduct = await db.Products.ToListAsync();
        Assert.Equal(2, retrievedProduct.Count);
    }

    [Fact]
    public async Task Update_Product_Should_Modify_Data()
    {
        using var db = GetDbContext();

        var product = new Product { Name = "Laptop", Price = 1000, Quantity = 5 };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        product.Price = 2000;

        await db.SaveChangesAsync();

        var updated = await db.Products.FirstAsync();

        Assert.Equal(2000, updated.Price);
    }

    [Fact]
    public async Task Delete_Product_Should_Remove_Product()
    {
        using var db = GetDbContext();

        var product = new Product { Name = "Laptop", Price = 1000, Quantity = 5 };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        Assert.Empty(db.Products);
    }
}