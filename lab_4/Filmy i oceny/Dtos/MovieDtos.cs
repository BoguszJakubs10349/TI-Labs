using System.ComponentModel.DataAnnotations;

namespace Lab04_Movies.Dtos;

public class MovieCreateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Range(1888, 3000)]
    public int Year { get; set; }
}

public class MovieRankingDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Year { get; set; }
    public decimal Avg_Score { get; set; }
    public int Votes { get; set; }
}
