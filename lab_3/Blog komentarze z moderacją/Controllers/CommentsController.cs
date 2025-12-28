using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab03_Blog.Data;
using Lab03_Blog.Dtos;

namespace Lab03_Blog.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly Db _db;
    public CommentsController(Db db) => _db = db;

    
    [HttpGet("pending")]
    public async Task<ActionResult<List<CommentDto>>> Pending()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            SELECT Id, PostId, Author, Body, CreatedAt, Approved
            FROM dbo.Comments
            WHERE Approved=0
            ORDER BY CreatedAt ASC;
        """, con);

        var list = new List<CommentDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new CommentDto
            {
                Id = r.GetInt32(0),
                PostId = r.GetInt32(1),
                Author = r.GetString(2),
                Body = r.GetString(3),
                CreatedAt = r.GetDateTime(4),
                Approved = r.GetBoolean(5)
            });
        }
        return Ok(list);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        
        var get = new SqlCommand("SELECT Approved FROM dbo.Comments WHERE Id=@Id;", con);
        get.Parameters.AddWithValue("@Id", id);
        var approvedObj = await get.ExecuteScalarAsync();
        if (approvedObj is null) return NotFound(new { error = "Comment not found" });

        var isApproved = Convert.ToBoolean(approvedObj);
        if (isApproved) return Conflict(new { error = "Already approved" });

        var cmd = new SqlCommand("""
            UPDATE dbo.Comments
            SET Approved=1
            WHERE Id=@Id;
        """, con);

        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();

        return Ok(new { ok = true });
    }
}
