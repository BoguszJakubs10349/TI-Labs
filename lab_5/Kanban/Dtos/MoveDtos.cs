using System.ComponentModel.DataAnnotations;

namespace Lab05_Kanban.Dtos;

public class MoveTaskDto
{
    [Required]
    public int Col_Id { get; set; }

    [Range(1, int.MaxValue)]
    public int Ord { get; set; }
}
