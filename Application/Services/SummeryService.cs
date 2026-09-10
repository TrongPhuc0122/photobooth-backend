using Application.DTOs;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Services;
public class SummeryService : ISummeryService
{
    private readonly IGenericRepository<Booths, Guid> _boothRepository;
    private readonly IGenericRepository<Invoice, int> _invoiceRepository;
    private readonly IGenericRepository<Branch, int> _branchRepository;

    public SummeryService(
        IGenericRepository<Booths, Guid> boothRepository,
        IGenericRepository<Invoice, int> invoiceRepository,
        IGenericRepository<Branch, int> branchRepository)
    {
        _boothRepository = boothRepository;
        _invoiceRepository = invoiceRepository;
        _branchRepository = branchRepository;
    }

    public ServiceResult<SummeryDto> GetSummeryInfor()
    {
        var now = DateTime.UtcNow;

        var branches = _branchRepository.GetAll().Count();
        var booths = _boothRepository.GetAll().Count();
        var activeBooths = _boothRepository.GetMulti(b => b.BoothHealth != null && b.BoothHealth.Status == Shared.Status.ONLINE).Count();
        var errorBooths = _boothRepository.GetMulti(b => b.BoothHealth != null && b.BoothHealth.Status == Shared.Status.ERROR).Count();
        var revenue = _invoiceRepository.GetAll()
                            .Sum(i => i.FinalPrice);

        var dto = new SummeryDto
        {
            Branch = branches,
            Booth = booths,
            ActiveBooth = activeBooths,
            ErrorBooth = errorBooths,
            Revenue = revenue
        };
        return ServiceResult<SummeryDto>.Success(dto);
    }
}