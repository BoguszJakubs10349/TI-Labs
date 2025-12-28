using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TI_Lab01_Library.Data;
using TI_Lab01_Library.Dtos;

namespace TI_Lab01_Library.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly Db _db;
    public BooksController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<BookDto>>> GetAll([FromQuery] string? author)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand($"""
            SELECT b.Id, b.Title, b.Author, b.Copies,
                   Available = b.Copies - ISNULL(x.ActiveLoans, 0)
            FROM dbo.Books b
            OUTER APPLY (
                SELECT ActiveLoans = COUNT(*)
                FROM dbo.Loans l
                WHERE l.BookId = b.Id AND l.ReturnDate IS NULL
            ) x
            WHERE (@Author IS NULL OR b.Author LIKE '%' + @Author + '%')
            ORDER BY b.Id DESC;
        """, con);

        cmd.Parameters.AddWithValue("@Author", (object?)author ?? DBNull.Value);

        var list = new List<BookDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new BookDto
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Author = r.GetString(2),
                Copies = r.GetInt32(3),
                Available = r.GetInt32(4)
            });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var copies = dto.Copies ?? 1;

        var cmd = new SqlCommand("""
            INSERT dbo.Books(Title, Author, Copies)
            OUTPUT INSERTED.Id
            VALUES(@Title, @Author, @Copies);
        """, con);

        cmd.Parameters.AddWithValue("@Title", dto.Title.Trim());
        cmd.Parameters.AddWithValue("@Author", dto.Author.Trim());
        cmd.Parameters.AddWithValue("@Copies", copies);

        var newId = (int)await cmd.ExecuteScalarAsync();

        Response.Headers.Location = $"/api/books/{newId}";
        return Created($"/api/books/{newId}", new { id = newId });
    }
}
