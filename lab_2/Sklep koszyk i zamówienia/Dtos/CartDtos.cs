using System.ComponentModel.DataAnnotations;

namespace Lab02_Shop.Dtos;

public class CartAddDto
{
    [Required]
    public int Product_Id { get; set; }

    [Range(1, int.MaxValue)]
    public int Qty { get; set; }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public decimal LineTotal { get; set; }
}

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}
