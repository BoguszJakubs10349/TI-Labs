using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TI_Lab01_Library.Data;
using TI_Lab01_Library.Dtos;

namespace TI_Lab01_Library.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly Db _db;
    public MembersController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<MemberDto>>> GetAll()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            SELECT Id, Name, Email
            FROM dbo.Members
            ORDER BY Id DESC;
        """, con);

        var list = new List<MemberDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new MemberDto
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Email = r.GetString(2)
            });
        }
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetById(int id)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            SELECT Id, Name, Email
            FROM dbo.Members
            WHERE Id = @Id;
        """, con);
        cmd.Parameters.AddWithValue("@Id", id);

        using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync()) return NotFound(new { error = "Member not found" });

        return Ok(new MemberDto
        {
            Id = r.GetInt32(0),
            Name = r.GetString(1),
            Email = r.GetString(2)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MemberCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        try
        {
            var cmd = new SqlCommand("""
                INSERT dbo.Members(Name, Email)
                OUTPUT INSERTED.Id
                VALUES(@Name, @Email);
            """, con);

            cmd.Parameters.AddWithValue("@Name", dto.Name.Trim());
            cmd.Parameters.AddWithValue("@Email", dto.Email.Trim());

            var newId = (int)await cmd.ExecuteScalarAsync();

            Response.Headers.Location = $"/api/members/{newId}";
            return Created($"/api/members/{newId}", new { id = newId });
        }
        catch (SqlException ex) when (SqlErrors.IsUniqueViolation(ex))
        {
            return Conflict(new { error = "Email must be unique" });
        }
    }
}
