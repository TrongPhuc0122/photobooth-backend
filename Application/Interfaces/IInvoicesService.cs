using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IInvoiceService : IGenericService<Invoice, InvoiceDto, CreateInvoicesDto, int>
    {
        Task<ServiceResult<InvoiceDto>> CreateAsync(int boothId, CreateInvoicesDto dto);
        ServiceResult<IEnumerable<AdminInvoiceDto>> GetByAdmin(string? branchCode, string? boothName, DateTime from, DateTime to);
    }
}
