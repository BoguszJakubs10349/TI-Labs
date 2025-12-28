using System.ComponentModel.DataAnnotations;

namespace TI_Lab01_Library.Dtos;

public sealed class BorrowDto
{
    [Required]
    public int Member_Id { get; set; }

    [Required]
    public int Book_Id { get; set; }

    [Range(1, 365)]
    public int? Days { get; set; } = 14;
}

public sealed class ReturnDto
{
    [Required]
    public int Loan_Id { get; set; }
}

public sealed class LoanListDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = "";
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
