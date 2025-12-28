using System.ComponentModel.DataAnnotations;

namespace TI_Lab01_Library.Dtos;

public sealed class BookCreateDto
{
    [Required, StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = "";

    [Required, StringLength(120, MinimumLength = 1)]
    public string Author { get; set; } = "";

    [Range(0, int.MaxValue)]
    public int? Copies { get; set; } = 1; // domyślnie 1
}

public sealed class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int Copies { get; set; }
    public int Available { get; set; }
}
