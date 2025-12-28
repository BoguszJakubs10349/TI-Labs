using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Lab05_Kanban.Data;
using Lab05_Kanban.Dtos;

namespace Lab05_Kanban.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly Db _db;
    public TasksController(Db db) => _db = db;

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();
        using var tx = con.BeginTransaction();

        
        var colExists = new SqlCommand("SELECT 1 FROM dbo.Columns WHERE Id=@Id;", con, tx);
        colExists.Parameters.AddWithValue("@Id", dto.Col_Id);
        if (await colExists.ExecuteScalarAsync() is null)
            return NotFound(new { error = "Column not found" });

        
        var nextOrdCmd = new SqlCommand("SELECT ISNULL(MAX(Ord),0)+1 FROM dbo.Tasks WHERE ColId=@ColId;", con, tx);
        nextOrdCmd.Parameters.AddWithValue("@ColId", dto.Col_Id);
        var nextOrd = Convert.ToInt32(await nextOrdCmd.ExecuteScalarAsync());

        var insert = new SqlCommand("""
            INSERT dbo.Tasks(Title, ColId, Ord)
            OUTPUT INSERTED.Id
            VALUES(@Title, @ColId, @Ord);
        """, con, tx);

        insert.Parameters.AddWithValue("@Title", dto.Title.Trim());
        insert.Parameters.AddWithValue("@ColId", dto.Col_Id);
        insert.Parameters.AddWithValue("@Ord", nextOrd);

        var id = Convert.ToInt32(await insert.ExecuteScalarAsync());
        tx.Commit();

        return Created($"/api/tasks/{id}", new { id, ord = nextOrd });
    }

   
    [HttpPost("{id:int}/move")]
    public async Task<IActionResult> Move(int id, [FromBody] MoveTaskDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();
        using var tx = con.BeginTransaction();

        var get = new SqlCommand("SELECT ColId, Ord FROM dbo.Tasks WHERE Id=@Id;", con, tx);
        get.Parameters.AddWithValue("@Id", id);
        using var r = await get.ExecuteReaderAsync();
        if (!r.Read())
            return NotFound(new { error = "Task not found" });

        var oldColId = r.GetInt32(0);
        var oldOrd = r.GetInt32(1);
        await r.DisposeAsync();

        var colExists = new SqlCommand("SELECT 1 FROM dbo.Columns WHERE Id=@Id;", con, tx);
        colExists.Parameters.AddWithValue("@Id", dto.Col_Id);
        if (await colExists.ExecuteScalarAsync() is null)
            return NotFound(new { error = "Target column not found" });

        if (oldColId == dto.Col_Id && oldOrd == dto.Ord)
        {
            tx.Commit();
            return Ok(new { ok = true });
        }

        var temp = new SqlCommand("UPDATE dbo.Tasks SET Ord=0 WHERE Id=@Id;", con, tx);
        temp.Parameters.AddWithValue("@Id", id);
        await temp.ExecuteNonQueryAsync();

        var closeGap = new SqlCommand("""
            UPDATE dbo.Tasks
            SET Ord = Ord - 1
            WHERE ColId=@OldColId AND Ord > @OldOrd;
        """, con, tx);
        closeGap.Parameters.AddWithValue("@OldColId", oldColId);
        closeGap.Parameters.AddWithValue("@OldOrd", oldOrd);
        await closeGap.ExecuteNonQueryAsync();

        var makeRoom = new SqlCommand("""
            UPDATE dbo.Tasks
            SET Ord = Ord + 1
            WHERE ColId=@NewColId AND Ord >= @NewOrd;
        """, con, tx);
        makeRoom.Parameters.AddWithValue("@NewColId", dto.Col_Id);
        makeRoom.Parameters.AddWithValue("@NewOrd", dto.Ord);
        await makeRoom.ExecuteNonQueryAsync();

        var place = new SqlCommand("""
            UPDATE dbo.Tasks
            SET ColId=@NewColId, Ord=@NewOrd
            WHERE Id=@Id;
        """, con, tx);
        place.Parameters.AddWithValue("@NewColId", dto.Col_Id);
        place.Parameters.AddWithValue("@NewOrd", dto.Ord);
        place.Parameters.AddWithValue("@Id", id);
        await place.ExecuteNonQueryAsync();

        tx.Commit();
        return Ok(new { ok = true });
    }
}
