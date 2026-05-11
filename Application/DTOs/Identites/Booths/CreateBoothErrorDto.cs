namespace Application.DTOs.Identites.Booths;

public class CreateBoothErrorDto
{
    public required string Cause { get; set; } = string.Empty;
    public string? ResolvedBy { get; set; }
}