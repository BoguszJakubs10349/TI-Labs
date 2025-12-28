using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab04_Movies.Data;
using Lab04_Movies.Dtos;

namespace Lab04_Movies.Controllers;

[ApiController]
[Route("api/ratings")]
public class RatingsController : ControllerBase
{
    private readonly Db _db;
    public RatingsController(Db db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RatingCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        
        var exists = new SqlCommand("SELECT 1 FROM dbo.Movies WHERE Id=@Id;", con);
        exists.Parameters.AddWithValue("@Id", dto.Movie_Id);
        if (await exists.ExecuteScalarAsync() is null)
            return NotFound(new { error = "Movie not found" });

        var cmd = new SqlCommand("""
            INSERT dbo.Ratings(MovieId, Score)
            OUTPUT INSERTED.Id
            VALUES(@MovieId, @Score);
        """, con);

        cmd.Parameters.AddWithValue("@MovieId", dto.Movie_Id);
        cmd.Parameters.AddWithValue("@Score", dto.Score);

        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return Created($"/api/ratings/{id}", new { id });
    }
}
