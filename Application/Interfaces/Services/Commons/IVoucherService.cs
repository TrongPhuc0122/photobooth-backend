using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces
{
    public interface IVoucherService : IGenericService<Voucher, VoucherDto, CreateVoucherDto, int>
    {
        
    }
}
