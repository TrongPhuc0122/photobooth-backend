using Application.DTOs.Identites;
using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IInvoiceService : IGenericService<Invoice, InvoiceDto, CreateInvoicesDto, int>
    {
        Task<ServiceResult<InvoiceDto>> CreateAsync(Guid boothId, CreateInvoicesDto dto);
        ServiceResult<PagedResult<InvoiceDto>> GetAll(InvoiceQueryParameters parameters);
        ServiceResult<PagedResult<DetailInvoiceDto>> GetDetailAll(InvoiceQueryParameters parameters);
        ServiceResult<DetailInvoiceDto> GetDetailById(int InvoiceId);
    }
}
