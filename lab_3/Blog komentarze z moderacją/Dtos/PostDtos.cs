using System.ComponentModel.DataAnnotations;

namespace Lab03_Blog.Dtos;

public class PostCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    public string Body { get; set; } = "";
}

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
