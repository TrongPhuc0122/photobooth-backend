using Application;

namespace Application.DTOs.Identites;

public class CreateBranchDto
{
    public required string BranchName { get; set; }
    public required string BranchCode {get; set; }
    public required string ManagerName { get; set; }
    public string? Creator { get; set; }    
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
}