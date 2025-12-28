using System.ComponentModel.DataAnnotations;

namespace Lab03_Blog.Dtos;

public class CommentCreateDto
{
    [Required, StringLength(100)]
    public string Author { get; set; } = "";

    [Required, StringLength(1000)]
    public string Body { get; set; } = "";
}

public class CommentDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Author { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool Approved { get; set; }
}
