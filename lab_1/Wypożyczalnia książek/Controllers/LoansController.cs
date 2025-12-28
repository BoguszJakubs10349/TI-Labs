using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TI_Lab01_Library.Data;
using TI_Lab01_Library.Dtos;

namespace TI_Lab01_Library.Controllers;

[ApiController]
[Route("api/loans")]
public class LoansController : ControllerBase
{
    private readonly Db _db;
    public LoansController(Db db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<LoanListDto>>> GetAll()
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        var cmd = new SqlCommand("""
            SELECT l.Id,
                   l.MemberId, m.Name,
                   l.BookId,   b.Title,
                   l.LoanDate, l.DueDate, l.ReturnDate
            FROM dbo.Loans l
            JOIN dbo.Members m ON m.Id = l.MemberId
            JOIN dbo.Books   b ON b.Id = l.BookId
            ORDER BY l.Id DESC;
        """, con);

        var list = new List<LoanListDto>();
        using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new LoanListDto
            {
                Id = r.GetInt32(0),
                MemberId = r.GetInt32(1),
                MemberName = r.GetString(2),
                BookId = r.GetInt32(3),
                BookTitle = r.GetString(4),
                LoanDate = r.GetDateTime(5),
                DueDate = r.GetDateTime(6),
                ReturnDate = r.IsDBNull(7) ? null : r.GetDateTime(7)
            });
        }
        return Ok(list);
    }

    [HttpPost("borrow")]
    public async Task<IActionResult> Borrow([FromBody] BorrowDto dto)
    {
        var days = dto.Days ?? 14;

        using var con = _db.CreateConnection();
        await con.OpenAsync();

        using var tx = con.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            // Sprawdź czy member istnieje
            var checkMember = new SqlCommand("SELECT 1 FROM dbo.Members WHERE Id=@Id;", con, tx);
            checkMember.Parameters.AddWithValue("@Id", dto.Member_Id);
            var memberExists = await checkMember.ExecuteScalarAsync();
            if (memberExists is null)
            {
                tx.Rollback();
                return NotFound(new { error = "Member not found" });
            }

            // Zablokuj rekord książki na czas sprawdzenia i wypożyczenia (żeby nie było wyścigu)
            var getCopies = new SqlCommand("""
                SELECT Copies
                FROM dbo.Books WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @BookId;
            """, con, tx);
            getCopies.Parameters.AddWithValue("@BookId", dto.Book_Id);

            var copiesObj = await getCopies.ExecuteScalarAsync();
            if (copiesObj is null)
            {
                tx.Rollback();
                return NotFound(new { error = "Book not found" });
            }

            var copies = (int)copiesObj;

            // Policz aktywne wypożyczenia
            var activeCmd = new SqlCommand("""
                SELECT COUNT(*)
                FROM dbo.Loans WITH (UPDLOCK, HOLDLOCK)
                WHERE BookId = @BookId AND ReturnDate IS NULL;
            """, con, tx);
            activeCmd.Parameters.AddWithValue("@BookId", dto.Book_Id);

            var active = (int)await activeCmd.ExecuteScalarAsync();

            if (active >= copies)
            {
                tx.Rollback();
                return Conflict(new { error = "No copies available" });
            }

            var loanDate = DateTime.UtcNow;
            var dueDate = loanDate.AddDays(days);

            var insert = new SqlCommand("""
                INSERT dbo.Loans(MemberId, BookId, LoanDate, DueDate)
                OUTPUT INSERTED.Id
                VALUES(@MemberId, @BookId, @LoanDate, @DueDate);
            """, con, tx);

            insert.Parameters.AddWithValue("@MemberId", dto.Member_Id);
            insert.Parameters.AddWithValue("@BookId", dto.Book_Id);
            insert.Parameters.AddWithValue("@LoanDate", loanDate);
            insert.Parameters.AddWithValue("@DueDate", dueDate);

            var newLoanId = (int)await insert.ExecuteScalarAsync();

            tx.Commit();

            Response.Headers.Location = $"/api/loans/{newLoanId}";
            return Created($"/api/loans/{newLoanId}", new { id = newLoanId });
        }
        catch
        {
            try { tx.Rollback(); } catch { /* ignore */ }
            throw;
        }
    }

    [HttpPost("return")]
    public async Task<IActionResult> Return([FromBody] ReturnDto dto)
    {
        using var con = _db.CreateConnection();
        await con.OpenAsync();

        using var tx = con.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            // Sprawdź stan wypożyczenia
            var get = new SqlCommand("""
                SELECT ReturnDate
                FROM dbo.Loans WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @Id;
            """, con, tx);
            get.Parameters.AddWithValue("@Id", dto.Loan_Id);

            using var r = await get.ExecuteReaderAsync();
            if (!await r.ReadAsync())
            {
                tx.Rollback();
                return NotFound(new { error = "Loan not found" });
            }

            var alreadyReturned = !r.IsDBNull(0);
            await r.DisposeAsync();

            if (alreadyReturned)
            {
                tx.Rollback();
                return Conflict(new { error = "Loan already returned" });
            }

            var upd = new SqlCommand("""
                UPDATE dbo.Loans
                SET ReturnDate = SYSUTCDATETIME()
                WHERE Id = @Id;
            """, con, tx);
            upd.Parameters.AddWithValue("@Id", dto.Loan_Id);

            await upd.ExecuteNonQueryAsync();
            tx.Commit();

            return Ok(new { ok = true });
        }
        catch
        {
            try { tx.Rollback(); } catch { /* ignore */ }
            throw;
        }
    }
}
