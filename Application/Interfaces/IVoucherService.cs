using Application.DTOs.Identites;
using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IVoucherService : IGenericService<Voucher, VoucherDto, CreateVoucherDto, int>
    {
        ServiceResult<PagedResult<VoucherDto>> GetAll(VoucherQueryParameters parameters);
        ServiceResult<IEnumerable<VoucherDto>> GetActiveVoucher();
        ServiceResult<VoucherDto> GetVoucherCode(string voucherCode);
    }
}
