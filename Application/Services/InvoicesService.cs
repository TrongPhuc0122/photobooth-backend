using Application.Interfaces;
using Application.Services.Commons;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;
using System.Xml.Serialization;

namespace Application.Services
{
    public class InvoiceService : GenericService<Invoice, InvoiceDto, CreateInvoicesDto, int>, IInvoiceService
    {
        private readonly IGenericRepository<Voucher, int> _voucherRepository;
        private readonly IGenericRepository<Booths, int> _boothRepository;
        public InvoiceService(
            IGenericRepository<Invoice, int> repository,
            IGenericRepository<Voucher, int> voucherRepository,
            IGenericRepository<Booths, int> boothRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
        ) : base(repository, mapper, unitOfWork)
        {
            _voucherRepository = voucherRepository;
            _boothRepository = boothRepository;
        }

        public async Task<ServiceResult<InvoiceDto>> CreateAsync(int boothId, CreateInvoicesDto dto)
        {
            try
            {   var booths = _boothRepository.GetSingleByCondition(b => b.BoothId == boothId);
                if(booths == null)
                {
                    return ServiceResult<InvoiceDto>.ValidationError($"Không tồn tại booth id = {boothId}");
                }
                Voucher? vouchers = dto.VoucherCode != null
                                    ? _voucherRepository.GetSingleByCondition(v => v.VoucherCode == dto.VoucherCode)
                                    : null;
                if (dto.VoucherCode != null && vouchers == null)
                        return ServiceResult<InvoiceDto>.ValidationError($"Không tồn tại voucher {dto.VoucherCode}");
                if(vouchers != null)
                {
                    if(DateTime.UtcNow > vouchers.EndDate)
                    {
                        return ServiceResult<InvoiceDto>.ValidationError("Voucher đã hết hạn");
                    }
                    else if(DateTime.UtcNow < vouchers.StartDate)
                    {
                        return ServiceResult<InvoiceDto>.ValidationError("Voucher chưa được áp dụng");
                    }
                    if(vouchers.UsageCount <= 0)
                    {
                        return ServiceResult<InvoiceDto>.ValidationError("Voucher đã hết lượt sử dụng");
                    }
                    vouchers.UsageCount--;
                    _voucherRepository.Update(vouchers);
                }
                
                var invoice = new Invoice
                {
                    InvoiceCode = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                    BoothId = boothId,
                    VoucherId = vouchers?.VoucherId,
                    Price = dto.Price,
                    FinalPrice = vouchers != null 
                                ? dto.Price * (decimal)(1.0f - vouchers.DiscountPercent)
                                : dto.Price,
                    FlowStatus = true,
                    PaymentMethod = dto.PaymentMethod,
                    CreatedAt = DateTime.UtcNow
                };
                _repository.Add(invoice);
                await _unitOfWork.SaveChangesAsync();
                var result = new InvoiceDto
                {
                    BoothName = booths.BoothName,
                    InvoiceCode = invoice.InvoiceCode,
                    Price = invoice.Price,
                    VoucherCode = invoice.Voucher?.VoucherCode,
                    DiscountPercent = invoice.Voucher?.DiscountPercent,
                    FinalPrice = invoice.FinalPrice,
                    CreatedAt = invoice.CreatedAt
                };
                return ServiceResult<InvoiceDto>.Created(result);
            }
            catch(Exception ex)
            {
                return ServiceResult<InvoiceDto>.InternalServerError($"Lỗi tạo hóa đơn {ex.Message}");
            }
        }
        public ServiceResult<IEnumerable<AdminInvoiceDto>> GetByAdmin(string? branchCode, string? boothName, DateTime from, DateTime to)
        {
            if(from > to)
            {
                return ServiceResult<IEnumerable<AdminInvoiceDto>>.ValidationError("Thời gian không hợp lệ");
            }            
            if((to - from).TotalDays > 365)
            {
                return ServiceResult<IEnumerable<AdminInvoiceDto>>.ValidationError("Khoảng thời gian không thể vượt quá 1 năm");
            }
            from = from.Date;
            to = to.Date.AddDays(1).AddTicks(-1);
            var invoices = _repository.GetMulti(i => i.CreatedAt >= from && i.CreatedAt < to &&
                                                    (boothName == null || i.Booth!.BoothName! == boothName) &&
                                                    (branchCode == null || i.Booth!.Branch!.BranchCode == branchCode),
                                                     includes: ["Booth", "Booth.Branch", "Voucher"]);
            var dto = invoices.Select(i => new AdminInvoiceDto
            {
                BranchCode = i.Booth?.Branch?.BranchCode ?? string.Empty,
                BoothName = i.Booth?.BoothName ?? string.Empty,
                InvoiceCode = i.InvoiceCode,
                PaymentMethod = i.PaymentMethod,
                Price = i.Price,
                FinalPrice = i.FinalPrice,
                VoucherCode = i.Voucher?.VoucherCode ?? string.Empty,
                DiscountPercent = i.Voucher?.DiscountPercent,
                CreatedAt = i.CreatedAt
            });
            return ServiceResult<IEnumerable<AdminInvoiceDto>>.Success(dto);
        }

    } 
}