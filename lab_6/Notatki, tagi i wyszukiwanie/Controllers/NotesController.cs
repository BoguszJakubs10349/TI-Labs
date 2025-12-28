using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab06_Notes.Data;
using Lab06_Notes.Dtos;

namespace Lab06_Notes.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly Db _db;
    public NotesController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<NoteListItemDto>>> Get([FromQuery] string? q = null, [FromQuery] string? tag = null)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var sql = """
            SELECT n.Id, n.Title,
                   LEFT(n.Body, 160) AS Snippet,
                   n.CreatedAt
            FROM dbo.Notes n
        """;

        if (!string.IsNullOrWhiteSpace(tag))
        {
            sql += """
                JOIN dbo.NoteTags nt ON nt.NoteId = n.Id
                JOIN dbo.Tags t ON t.Id = nt.TagId
            """;
        }

        sql += " WHERE 1=1 ";

        if (!string.IsNullOrWhiteSpace(q))
        {
            sql += " AND (n.Title LIKE @Q OR n.Body LIKE @Q) ";
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            sql += " AND t.Name = @Tag ";
        }

        sql += " ORDER BY n.CreatedAt DESC, n.Id DESC;";

        var cmd = new SqlCommand(sql, con);

        if (!string.IsNullOrWhiteSpace(q))
            cmd.Parameters.AddWithValue("@Q", "%" + q + "%");
        if (!string.IsNullOrWhiteSpace(tag))
            cmd.Parameters.AddWithValue("@Tag", tag.Trim());

        var list = new List<NoteListItemDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new NoteListItemDto
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Snippet = r.GetString(2),
                CreatedAt = r.GetDateTime(3)
            });
        }
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NoteCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            INSERT dbo.Notes(Title, Body)
            OUTPUT INSERTED.Id
            VALUES(@Title, @Body);
        """, con);

        cmd.Parameters.AddWithValue("@Title", dto.Title.Trim());
        cmd.Parameters.AddWithValue("@Body", dto.Body);

        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        Response.Headers.Location = $"/api/notes/{id}";
        return Created($"/api/notes/{id}", new { id });
    }

    [HttpPost("{id:int}/tags")]
    public async Task<IActionResult> SetTags(int id, [FromBody] NoteTagsSetDto dto)
    {
        var tags = dto.Tags
            .Select(t => (t ?? "").Trim())
            .Where(t => t.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        using var con = _db.CreateConnection();
        await con.OpenAsync();
        using var tx = con.BeginTransaction();

        var exists = new SqlCommand("SELECT 1 FROM dbo.Notes WHERE Id=@Id;", con, tx);
        exists.Parameters.AddWithValue("@Id", id);
        if (await exists.ExecuteScalarAsync() is null)
            return NotFound(new { error = "Note not found" });

        foreach (var tagName in tags)
        {
            int tagId;
            var getTag = new SqlCommand("SELECT Id FROM dbo.Tags WHERE Name=@Name;", con, tx);
            getTag.Parameters.AddWithValue("@Name", tagName);
            var existingId = await getTag.ExecuteScalarAsync();

            if (existingId is null)
            {
                var insertTag = new SqlCommand("""
                    INSERT dbo.Tags(Name)
                    OUTPUT INSERTED.Id
                    VALUES(@Name);
                """, con, tx);
                insertTag.Parameters.AddWithValue("@Name", tagName);

                try
                {
                    tagId = Convert.ToInt32(await insertTag.ExecuteScalarAsync());
                }
                catch (SqlException)
                {
                    var again = new SqlCommand("SELECT Id FROM dbo.Tags WHERE Name=@Name;", con, tx);
                    again.Parameters.AddWithValue("@Name", tagName);
                    tagId = Convert.ToInt32(await again.ExecuteScalarAsync());
                }
            }
            else
            {
                tagId = Convert.ToInt32(existingId);
            }

            var link = new SqlCommand("""
                IF NOT EXISTS (SELECT 1 FROM dbo.NoteTags WHERE NoteId=@NoteId AND TagId=@TagId)
                INSERT dbo.NoteTags(NoteId, TagId) VALUES(@NoteId, @TagId);
            """, con, tx);

            link.Parameters.AddWithValue("@NoteId", id);
            link.Parameters.AddWithValue("@TagId", tagId);
            await link.ExecuteNonQueryAsync();
        }

        tx.Commit();
        return Ok(new { ok = true, linked = tags.Count });
    }
}
