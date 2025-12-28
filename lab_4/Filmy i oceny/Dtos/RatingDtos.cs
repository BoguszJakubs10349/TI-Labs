using System.ComponentModel.DataAnnotations;

namespace Lab04_Movies.Dtos;

public class RatingCreateDto
{
    [Required]
    public int Movie_Id { get; set; }

    [Range(1, 5)]
    public int Score { get; set; }
}
