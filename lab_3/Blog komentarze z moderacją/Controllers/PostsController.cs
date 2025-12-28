using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab03_Blog.Data;
using Lab03_Blog.Dtos;

namespace Lab03_Blog.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly Db _db;
    public PostsController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetPosts()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            SELECT Id, Title, Body, CreatedAt
            FROM dbo.Posts
            ORDER BY Id DESC;
        """, con);

        var list = new List<PostDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new PostDto
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Body = r.GetString(2),
                CreatedAt = r.GetDateTime(3)
            });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            INSERT dbo.Posts(Title, Body)
            OUTPUT INSERTED.Id
            VALUES(@Title, @Body);
        """, con);

        cmd.Parameters.AddWithValue("@Title", dto.Title.Trim());
        cmd.Parameters.AddWithValue("@Body", dto.Body);

        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        Response.Headers.Location = $"/api/posts/{id}";
        return Created($"/api/posts/{id}", new { id });
    }

    
    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<List<CommentDto>>> GetApprovedComments(int id)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        
        var exists = new SqlCommand("SELECT 1 FROM dbo.Posts WHERE Id=@Id;", con);
        exists.Parameters.AddWithValue("@Id", id);
        if (await exists.ExecuteScalarAsync() is null)
            return NotFound(new { error = "Post not found" });

        var cmd = new SqlCommand("""
            SELECT Id, PostId, Author, Body, CreatedAt, Approved
            FROM dbo.Comments
            WHERE PostId=@PostId AND Approved=1
            ORDER BY CreatedAt ASC;
        """, con);

        cmd.Parameters.AddWithValue("@PostId", id);

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

    
    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] CommentCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        
        var exists = new SqlCommand("SELECT 1 FROM dbo.Posts WHERE Id=@Id;", con);
        exists.Parameters.AddWithValue("@Id", id);
        if (await exists.ExecuteScalarAsync() is null)
            return NotFound(new { error = "Post not found" });

        var cmd = new SqlCommand("""
            INSERT dbo.Comments(PostId, Author, Body)
            OUTPUT INSERTED.Id
            VALUES(@PostId, @Author, @Body);
        """, con);

        cmd.Parameters.AddWithValue("@PostId", id);
        cmd.Parameters.AddWithValue("@Author", dto.Author.Trim());
        cmd.Parameters.AddWithValue("@Body", dto.Body);

        var commentId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return Created($"/api/comments/{commentId}", new { id = commentId, approved = 0 });
    }
}
