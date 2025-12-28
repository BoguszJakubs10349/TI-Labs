using System.ComponentModel.DataAnnotations;

namespace Lab06_Notes.Dtos;

public class NoteCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    public string Body { get; set; } = "";
}

public class NoteListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Snippet { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class NoteTagsSetDto
{
    [Required]
    public List<string> Tags { get; set; } = new();
}
