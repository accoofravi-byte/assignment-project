using AssignmentApi.Data;
using AssignmentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
[AllowAnonymous]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        Product product)
    {
        var existing = await _context.Products.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Quantity = product.Quantity;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}