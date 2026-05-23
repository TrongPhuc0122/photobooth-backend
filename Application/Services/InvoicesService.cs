using Application.Interfaces;
using Application.Services.Commons;
using Application.DTOs.Identites;
using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;
using Shared;

namespace Application.Services
{
    public class InvoiceService : GenericService<Invoice, InvoiceDto, CreateInvoicesDto, int>, IInvoiceService
    {
        private readonly IGenericRepository<Voucher, int> _voucherRepository;
        private readonly IGenericRepository<Booths, Guid> _boothRepository;
        public InvoiceService(
            IGenericRepository<Invoice, int> repository,
            IGenericRepository<Voucher, int> voucherRepository,
            IGenericRepository<Booths, Guid> boothRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
        ) : base(repository, mapper, unitOfWork)
        {
            _voucherRepository = voucherRepository;
            _boothRepository = boothRepository;
        }

        public async Task<ServiceResult<InvoiceDto>> CreateAsync(Guid boothId, CreateInvoicesDto dto)
        {
            var check = CheckExist(boothId);
            if (!check.IsSuccess)
            {
                return ServiceResult<InvoiceDto>.InternalServerError(check.Message);
            }
            Voucher? voucher = null;
            if (dto.VoucherCode != null)
            {
                var status = VoucherStatus(dto.VoucherCode);
                if (status == UsedStatus.UpComing || status == UsedStatus.Expired)
                    return ServiceResult<InvoiceDto>.ValidationError("Voucher không hợp lệ");

                voucher = _voucherRepository.GetSingleByCondition(v => v.VoucherCode == dto.VoucherCode);
                if (status == UsedStatus.Active)
                {
                    voucher.UsageCount++;
                    _voucherRepository.Update(voucher);
                }
            }
            try
            {
                Invoice invoice = new Invoice
                {
                    InvoiceCode = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                    BoothId = boothId,
                    VoucherId = voucher?.VoucherId,
                    Price = dto.Price,
                    FinalPrice = voucher != null 
                                    ? dto.Price * (decimal)(1.0f - voucher.DiscountPercent)
                                    : dto.Price,
                    FlowStatus = true,
                    PaymentMethod = dto.PaymentMethod,
                    CreatedAt = DateTime.UtcNow
                };
                _repository.Add(invoice);
                await _unitOfWork.SaveChangesAsync();
                var result = new InvoiceDto
                {
                    Infor = {
                        Price = invoice.Price,
                        DiscountPercent = voucher != null ? voucher.DiscountPercent : 0,
                        FinalPrice = invoice.FinalPrice
                    },
                    InvoiceCode = invoice.InvoiceCode,
                    VoucherCode = voucher?.VoucherCode,
                    CreatedAt = invoice.CreatedAt
                };
                return ServiceResult<InvoiceDto>.Success(result);
            }
            catch(Exception ex)
            {
                return ServiceResult<InvoiceDto>.InternalServerError($"Lỗi tạo Invoice: {ex.Message}");
            }
        }
        public ServiceResult<PagedResult<InvoiceDto>> GetAll(InvoiceQueryParameters parameters)
        {
            var genericParams = parameters.ToGenericQueryParameters();
            string[] searchProperties = {"InvoiceCode"};
            string[] includes = {"Booth", "Voucher"};
            var pagedEntities = _repository.GetPaged(genericParams, searchProperties, includes);
            var now = DateTime.UtcNow;
            var result = pagedEntities.Items.Select(i => new InvoiceDto
            {
                Infor =
                {
                    Price = i.Price,
                    DiscountPercent = i.Voucher != null ? i.Voucher.DiscountPercent * 100.0f : 0,
                    FinalPrice = i.FinalPrice
                },
                VoucherCode = i.Voucher?.VoucherCode,
                InvoiceCode = i.InvoiceCode,
                CreatedAt = i.CreatedAt
            });
            var pagedResult = new PagedResult<InvoiceDto>(
                result,
                pagedEntities.TotalCount,
                pagedEntities.Index,
                pagedEntities.PageSize
            );
            return ServiceResult<PagedResult<InvoiceDto>>.Success(pagedResult);
        }
        public ServiceResult<PagedResult<DetailInvoiceDto>> GetDetailAll(InvoiceQueryParameters parameters)
        {
            var genericParams = parameters.ToGenericQueryParameters();
            string[] searchProperties = {"InvoiceCode"};
            string[] includes = {"Booth.Branch","Booth", "Voucher"};
            var pagedEntities = _repository.GetPaged(genericParams, searchProperties, includes);
            var now = DateTime.UtcNow;
            try
            {
                var result = pagedEntities.Items.Select(i => new DetailInvoiceDto
                {
                    BoothId = i.BoothId,
                    Infor =
                    {
                        InvoiceId = i.InvoiceId,
                        InvoiceCode = i.InvoiceCode,
                        BranchCode = i.Booth!.Branch.BranchCode,
                        BoothName = i.Booth.BoothName
                    },
                    Payment =
                    {
                        Price = i.Price,
                        VoucherCOde = i.Voucher?.VoucherCode,
                        DiscountPercent = i.Voucher?.DiscountPercent * 100.0f,
                        FinalPrice = i.FinalPrice,
                        PaymentMethod = i.PaymentMethod
                    },
                    CreatedAt = i.CreatedAt
                });
                var pagedResult = new PagedResult<DetailInvoiceDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );
                return ServiceResult<PagedResult<DetailInvoiceDto>>.Success(pagedResult);   
            }
            catch(Exception ex)
            {
                return ServiceResult<PagedResult<DetailInvoiceDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        public ServiceResult<DetailInvoiceDto> GetDetailById(int InvoiceId)
        {
            Invoice invoice;
            try
            {
                invoice = _repository.GetSingleById(InvoiceId);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<DetailInvoiceDto>.NotFound($"Không tồn tại InvoideId = {InvoiceId}");
            }
            try
            {
                var reult = new DetailInvoiceDto
                {
                    BoothId = invoice.BoothId,
                        Infor =
                        {
                            InvoiceId = invoice.InvoiceId,
                            InvoiceCode = invoice.InvoiceCode,
                            BranchCode = invoice.Booth!.Branch.BranchCode,
                            BoothName = invoice.Booth.BoothName
                        },
                        Payment =
                        {
                            Price = invoice.Price,
                            VoucherCOde = invoice.Voucher?.VoucherCode,
                            DiscountPercent = invoice.Voucher?.DiscountPercent * 100.0f,
                            FinalPrice = invoice.FinalPrice,
                            PaymentMethod = invoice.PaymentMethod
                        },
                        CreatedAt = invoice.CreatedAt
                };
                return ServiceResult<DetailInvoiceDto>.Success(reult);
            }
            catch(Exception ex)
            {
                return ServiceResult<DetailInvoiceDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        // VALIDATOR
        public ServiceResult CheckExist(Guid boothId)
        {
            Booths booths;
            try
            {
                booths = _boothRepository.GetSingleById(boothId);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.InternalServerError($"Không tồn tại BoothId = {boothId}");
            }
            return ServiceResult.Success();
        }
        public UsedStatus VoucherStatus(string? voucherCode)
        {
            if (voucherCode == null) return UsedStatus.NoVoucher;

            DateTime now = DateTime.UtcNow;
            Voucher voucher = _voucherRepository.GetSingleByCondition(v => v.VoucherCode == voucherCode);

            if (voucher.StartDate > now) return UsedStatus.UpComing;
            if (voucher.EndDate < now || (voucher.UsageLimit != null && voucher.UsageCount >= voucher.UsageLimit)) return UsedStatus.Expired;
            return UsedStatus.Active;
        }
    }
}