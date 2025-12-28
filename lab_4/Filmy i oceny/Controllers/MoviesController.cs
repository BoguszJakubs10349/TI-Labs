using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab04_Movies.Data;
using Lab04_Movies.Dtos;

namespace Lab04_Movies.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly Db _db;
    public MoviesController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<MovieRankingDto>>> Get([FromQuery] int? year = null, [FromQuery] int? limit = null)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        
        var sql = """
            SELECT Id, Title, [Year],
                   ISNULL(AvgScore, 0) AS AvgScore,
                   Votes
            FROM dbo.vMoviesRanking
        """;

        if (year is not null) sql += " WHERE [Year] = @Year";
        sql += " ORDER BY ISNULL(AvgScore, 0) DESC, Votes DESC, Id DESC";
        if (limit is not null) sql += " OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;";

        var cmd = new SqlCommand(sql, con);
        if (year is not null) cmd.Parameters.AddWithValue("@Year", year.Value);
        if (limit is not null) cmd.Parameters.AddWithValue("@Limit", limit.Value);

        var list = new List<MovieRankingDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new MovieRankingDto
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Year = r.GetInt32(2),
                Avg_Score = r.GetDecimal(3),
                Votes = r.GetInt32(4)
            });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MovieCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            INSERT dbo.Movies(Title, [Year])
            OUTPUT INSERTED.Id
            VALUES(@Title, @Year);
        """, con);

        cmd.Parameters.AddWithValue("@Title", dto.Title.Trim());
        cmd.Parameters.AddWithValue("@Year", dto.Year);

        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        Response.Headers.Location = $"/api/movies/{id}";
        return Created($"/api/movies/{id}", new { id });
    }

    
    [HttpGet("top")]
    public async Task<ActionResult<List<MovieRankingDto>>> Top([FromQuery] int limit = 5)
        => await Get(null, limit);
}
