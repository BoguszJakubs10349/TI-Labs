using System.ComponentModel.DataAnnotations;

namespace TI_Lab01_Library.Dtos;

public class ProductCreateDto
{
    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
