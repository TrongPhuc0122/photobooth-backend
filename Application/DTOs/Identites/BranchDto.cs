using AutoMapper.Internal.Mappers;
using Shared;
namespace Application.DTOs.Identites;

public class BranchDto
{
    public BranchBasicInfor Infor { get; set; } = new();
    public string BranchCode { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string? Creator { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Status Status { get; set; }
    public DateTime CreateAt = DateTime.UtcNow;
    public int TotalBooths { get; set; }
    public int ActiveBooths { get; set; }
    public decimal MonthlyRevenue { get; set; }
}
public class BranchBasicInfor
{
    public int Id { get; set; }
    public string BranchName { get; set; } = string.Empty;
}