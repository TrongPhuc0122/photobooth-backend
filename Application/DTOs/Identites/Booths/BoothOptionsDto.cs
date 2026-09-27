namespace Application.DTOs.Identites.Booths;

public class BoothOptionDto
{
    public Guid BoothId { get; set; }
    public string BoothName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
}