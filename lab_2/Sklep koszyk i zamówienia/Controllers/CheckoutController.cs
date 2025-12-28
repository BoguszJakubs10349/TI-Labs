using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab02_Shop.Data;
using Lab02_Shop.Dtos;
using Lab02_Shop.Services;

namespace Lab02_Shop.Controllers;

[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly Db _db;
    private readonly CartService _cart;

    public CheckoutController(Db db, CartService cart)
    {
        _db = db;
        _cart = cart;
    }

    [HttpPost("api/checkout")]
    public async Task<IActionResult> Checkout()
    {
        var items = _cart.GetAll();
        if (!items.Any())
            return Conflict(new { error = "Cart is empty" });

        using var con = _db.CreateConnection();
        await con.OpenAsync();
        using var tx = con.BeginTransaction();

        try
        {
            var orderCmd = new SqlCommand(
                    "INSERT dbo.Orders OUTPUT INSERTED.Id DEFAULT VALUES;",
                        con, tx);

            var orderId = (int)await orderCmd.ExecuteScalarAsync();
            decimal total = 0;

            foreach (var (pid, qty) in items)
            {
                var priceCmd = new SqlCommand(
                    "SELECT Price FROM dbo.Products WHERE Id=@Id",
                    con, tx);
                priceCmd.Parameters.AddWithValue("@Id", pid);

                var price = (decimal)await priceCmd.ExecuteScalarAsync();
                total += price * qty;

                var itemCmd = new SqlCommand("""
                    INSERT dbo.OrderItems(OrderId, ProductId, Qty, Price)
                    VALUES(@O, @P, @Q, @Pr)
                """, con, tx);

                itemCmd.Parameters.AddWithValue("@O", orderId);
                itemCmd.Parameters.AddWithValue("@P", pid);
                itemCmd.Parameters.AddWithValue("@Q", qty);
                itemCmd.Parameters.AddWithValue("@Pr", price);

                await itemCmd.ExecuteNonQueryAsync();
            }

            tx.Commit();
            _cart.Clear();

            return Created("/api/orders/" + orderId,
                new CheckoutResultDto { Order_Id = orderId, Total = total });
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
