using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;

namespace Application.Services;
public class DashBoradService : IDashBoardService
{
    private readonly IGenericRepository<Booths, Guid> _boothrepository;
    private readonly IGenericRepository<Invoice, int> _invoiceRepository;
    public DashBoradService(
        IGenericRepository<Booths, Guid> BoothRepository,
        IGenericRepository<Invoice, int> InvoiceRepository,
        IMapper mapper
    )
    {
        _boothrepository = BoothRepository;
        _invoiceRepository = InvoiceRepository;
    }
    public async Task<ServiceResult<DashBoradDto>> GetDashboard(DateTime from, DateTime to)
    {
        IEnumerable<Invoice> invoice;
        try
        {
            invoice = _invoiceRepository.GetMulti(i => i.CreatedAt >= from &&
                                                        i.CreatedAt <= to);   
        }
        catch(Exception ex)
        {
            return ServiceResult<DashBoradDto>.InternalServerError($"Lỗi truy vấn hóa đơn: {ex.Message}");
        }
        IEnumerable<Booths> booths;
        try
        {
            booths = _boothrepository.GetMulti(
    b => b.BoothErrors.Any(e => e.IsFixed == false) ||
        (b.BoothResources != null && (
            (b.BoothResources.PaperMax > 0 && (float)b.BoothResources.PaperCount / (float)b.BoothResources.PaperMax <= 0.1) ||
            (b.BoothResources.RibbonMax > 0 && (float)b.BoothResources.RibbonCount / (float)b.BoothResources.RibbonMax <= 0.1)
        )),
    includes: ["BoothErrors", "BoothHealth", "BoothResources"]
);
        }
        catch(Exception ex)
        {
            return ServiceResult<DashBoradDto>.InternalServerError($"Lỗi truy cấn tài nguyên: {ex.Message}");
        }
        var dataPoint = invoice
            .GroupBy(i => i.CreatedAt.Date)
            .Select(g => new RevenueDataPoint
            {
                Date = g.Key,
                FinalPrice = g.Sum(i => i.FinalPrice)
            })
            .OrderBy(d => d.Date)
            .ToList();
        var BoothInfor = booths
            .OrderBy(b => b.BoothErrors.Any(e => !e.IsFixed) ? 0 : 1)
            .Select(b => new BoothInfor
            {
                BoothId = b.BoothId,
                BoothName = b.BoothName,
                status = b.BoothHealth!.Status,
                PaperCount = b.BoothResources?.PaperCount ?? 0,
                RibbonCount = b.BoothResources?.RibbonCount ?? 0
            }).ToList();
        try
        {
            var board = new DashBoradDto
            {
                Revenue = new RevenueInfor{
                    From = from,
                    To = to,
                    FinalPrice = dataPoint.Sum(d => d.FinalPrice),
                    DataPoints = dataPoint
                },
                BoothStatuses = BoothInfor
            };
            return ServiceResult<DashBoradDto>.Success(board);
        }
        catch(Exception ex)
        {
            return ServiceResult<DashBoradDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }
}