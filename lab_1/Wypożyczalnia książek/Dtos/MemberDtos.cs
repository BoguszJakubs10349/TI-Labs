using System.ComponentModel.DataAnnotations;

namespace TI_Lab01_Library.Dtos;

public sealed class MemberCreateDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";

    [Required, StringLength(200), EmailAddress]
    public string Email { get; set; } = "";
}

public sealed class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
