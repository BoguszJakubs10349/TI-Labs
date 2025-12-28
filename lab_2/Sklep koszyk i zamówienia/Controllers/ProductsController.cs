using Lab02_Shop.Data;
using Lab02_Shop.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TI_Lab01_Library.Dtos;

namespace Lab02_Shop.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly Db _db;
    public ProductsController(Db db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand(
            "SELECT Id, Name, Price FROM dbo.Products", con);

        var list = new List<ProductDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new ProductDto
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Price = r.GetDecimal(2)
            });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            INSERT dbo.Products(Name, Price)
            OUTPUT INSERTED.Id
            VALUES(@Name, @Price)
        """, con);

        cmd.Parameters.AddWithValue("@Name", dto.Name);
        cmd.Parameters.AddWithValue("@Price", dto.Price);

        var id = (int)await cmd.ExecuteScalarAsync();
        return Created($"/api/products/{id}", new { id });
    }
}
