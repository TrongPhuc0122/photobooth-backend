using System.Reflection.Metadata.Ecma335;
using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;

namespace Application.Services
{
    public class VoucherService : GenericService<Voucher, VoucherDto, CreateVoucherDto, int>, IVoucherService
    {
        private readonly IGenericRepository<Invoice, int> _invoiceRepository;
        public VoucherService(
            IGenericRepository<Voucher, int> repository,
            IGenericRepository<Invoice, int> invoiceRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
        ) : base (repository, mapper, unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
        }

        public override async Task<ServiceResult<VoucherDto>> CreateAsync(CreateVoucherDto dto)
        {
            try
            {
                var voucher = new Voucher
                {
                    VoucherCode = dto.VoucherCode,
                    Purpose = dto.Purpose,
                    DiscountPercent = dto.DiscountPercent/100.0f,
                    CreatedAt = DateTime.UtcNow,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    UsageLimit = dto.UsageLimit,
                    UsageCount = 0
                };
                _repository.Add(voucher);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.Map<VoucherDto>(voucher);
                return ServiceResult<VoucherDto>.Created(result);
            }
            catch(Exception ex)
            {
                return ServiceResult<VoucherDto>.InternalServerError($"Lỗi tạo voucher {ex.Message}");
            }
        }
    }
}

