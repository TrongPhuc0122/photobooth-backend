namespace Application.DTOs.Identites.Booths;
public class BoothErrorDto
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Cause { get; set; } = string.Empty;
    public string? Solution { get; set; }
    public string? ResolvedBy { get; set; }
    public bool IsFixed { get; set; }
    public DateTime CreateAt { get; set; }
}