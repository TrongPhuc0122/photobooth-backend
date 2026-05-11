namespace Application.DTOs.Identites.Booths;

public class CreateBoothDto
{
    public required string BranchCode { get; set; } = string.Empty;
    public required string BoothName { get; set; } = string.Empty;
    public required string BoothIp { get; set; } = string.Empty;
    public required string Brand { get; set; } = string.Empty;
    public string? Creator { get; set; }
}