using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab05_Kanban.Data;
using Lab05_Kanban.Dtos;

namespace Lab05_Kanban.Controllers;

[ApiController]
[Route("api/board")]
public class BoardController : ControllerBase
{
    private readonly Db _db;
    public BoardController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<BoardDto>> Get()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var board = new BoardDto();

        using (var cmd = new SqlCommand("SELECT Id, Name, Ord FROM dbo.Columns ORDER BY Ord ASC;", con))
        using (var r = await cmd.ExecuteReaderAsync())
        {
            while (await r.ReadAsync())
            {
                board.Cols.Add(new ColumnDto
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Ord = r.GetInt32(2)
                });
            }
        }

        using (var cmd = new SqlCommand("SELECT Id, Title, ColId, Ord FROM dbo.Tasks ORDER BY ColId ASC, Ord ASC;", con))
        using (var r = await cmd.ExecuteReaderAsync())
        {
            while (await r.ReadAsync())
            {
                board.Tasks.Add(new TaskDto
                {
                    Id = r.GetInt32(0),
                    Title = r.GetString(1),
                    ColId = r.GetInt32(2),
                    Ord = r.GetInt32(3)
                });
            }
        }

        return Ok(board);
    }
}
