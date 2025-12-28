using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab02_Shop.Data;
using Lab02_Shop.Dtos;
using Lab02_Shop.Services;

namespace Lab02_Shop.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly Db _db;
    private readonly CartService _cart;

    public CartController(Db db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cart = new CartDto();
        var items = _cart.GetAll();

        using var con = _db.CreateConnection();
        await con.OpenAsync();

        foreach (var (pid, qty) in items)
        {
            var cmd = new SqlCommand(
                "SELECT Name, Price FROM dbo.Products WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", pid);

            using var r = await cmd.ExecuteReaderAsync();
            if (!r.Read()) continue;

            var price = r.GetDecimal(1);
            var line = price * qty;

            cart.Items.Add(new CartItemDto
            {
                ProductId = pid,
                Name = r.GetString(0),
                Price = price,
                Qty = qty,
                LineTotal = line
            });

            cart.Total += line;
        }
        return Ok(cart);
    }

    [HttpPost("add")]
    public IActionResult Add(CartAddDto dto)
    {
        _cart.Add(dto.Product_Id, dto.Qty);
        return Ok();
    }
}
