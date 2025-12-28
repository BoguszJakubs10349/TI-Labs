using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab06_Notes.Data;
using Lab06_Notes.Dtos;

namespace Lab06_Notes.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly Db _db;
    public TagsController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("SELECT Id, Name FROM dbo.Tags ORDER BY Name ASC;", con);
        var list = new List<TagDto>();

        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new TagDto
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            });
        }
        return Ok(list);
    }
}
