using System.ComponentModel.DataAnnotations;

namespace Lab05_Kanban.Dtos;

public class TaskCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    public int Col_Id { get; set; }
}

public class ColumnDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Ord { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int ColId { get; set; }
    public int Ord { get; set; }
}

public class BoardDto
{
    public List<ColumnDto> Cols { get; set; } = new();
    public List<TaskDto> Tasks { get; set; } = new();
}
