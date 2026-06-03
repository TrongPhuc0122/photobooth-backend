using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Shared;
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
        ) : base(repository, mapper, unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
        }

        public override async Task<ServiceResult<VoucherDto>> CreateAsync(CreateVoucherDto dto)
        {
            if((dto.UsageLimit == null || dto.UsageLimit == 0) && dto.EndDate == null)
            {
                return ServiceResult<VoucherDto>.InternalServerError("Không được bỏ trống UsageLimit và EndDate");
            }
            try
            {
                var voucher = _repository.CheckContains(v => v.VoucherCode == dto.VoucherCode);
                if (voucher)
                {
                    return ServiceResult<VoucherDto>.ValidationError("Đã tồn tại voucher");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<VoucherDto>.InternalServerError($"Lỗi tạo voucher 000: {ex.Message}");

            }
            if(!VoucherDiscountCheck(dto)) return ServiceResult<VoucherDto>.InternalServerError("Lỗi tạo voucher 001");
            try
            {
                var voucher = new Voucher
                {
                    BranchId = dto.BranchId,
                    VoucherCode = dto.VoucherCode,
                    Purpose = dto.Purpose,
                    DiscountPercent = dto.DiscountPercent / 100.0f,
                    CreatedAt = DateTime.UtcNow,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    UsageLimit = dto.UsageLimit,
                    UsageCount = 0,
                    Creator = dto.Creator
                };
                _repository.Add(voucher);
                await _unitOfWork.SaveChangesAsync();
                var result = new VoucherDto
                {
                    Infor =
                    {
                        VoucherId = voucher.VoucherId,
                        VoucherCode = voucher.VoucherCode,
                        DiscountPercent = voucher.DiscountPercent * 100.0f
                    },
                    BranchId = voucher.BranchId,
                    Purpose = voucher.Purpose,
                    StartDate = voucher.StartDate,
                    EndDate = voucher.EndDate,
                    UsageLimit = voucher.UsageLimit,
                    UsageCount = voucher.UsageCount,
                    BeUsed = VoucherStatus(voucher)
                };
                return ServiceResult<VoucherDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<VoucherDto>.InternalServerError($"Lỗi tạo voucher 002: {ex.Message}");
            }
        }   
        public ServiceResult<PagedResult<VoucherDto>> GetAll(VoucherQueryParameters parameters)
        {
            try
            {
                var genericParams = parameters.ToGenericQueryParameters();
                string[] searchProperties = {"Purpose", "VoucherCode"};
                string[] includes = {"Branch"};
                var pagedEntities = _repository.GetPaged(genericParams, searchProperties, includes);
                var filtered = pagedEntities.Items.AsEnumerable();
                if(parameters.usedStatus.HasValue){
                    var now = DateTime.UtcNow;
                    filtered = parameters.usedStatus switch
                    {
                        UsedStatus.Active => filtered.Where(v => v.UsageLimit == null || v.UsageCount < v.UsageLimit),
                        UsedStatus.Expired => filtered.Where(v => v.UsageCount >= v.UsageLimit && v.UsageLimit != null),
                        _ => filtered 
                    };
                }
                var result = pagedEntities.Items.Select(voucher => new VoucherDto
                {
                    Infor =
                    {
                        VoucherId = voucher.VoucherId,
                        VoucherCode = voucher.VoucherCode,
                        DiscountPercent = voucher.DiscountPercent * 100.0f
                    },
                    BranchId = voucher.BranchId,
                    Purpose = voucher.Purpose,
                    StartDate = voucher.StartDate,
                    EndDate = voucher.EndDate,
                    UsageLimit = voucher.UsageLimit,
                    UsageCount = voucher.UsageCount,
                    BeUsed = VoucherStatus(voucher)
                });
                var pagedResult = new PagedResult<VoucherDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );
                return ServiceResult<PagedResult<VoucherDto>>.Success(pagedResult);
            }
            catch(Exception ex)
            {
                return ServiceResult<PagedResult<VoucherDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        public ServiceResult<IEnumerable<VoucherDto>> GetActiveVoucher()
        {
            var vouchers = _repository.GetMulti(v => v.StartDate <= DateTime.UtcNow &&
                                                     v.EndDate >= DateTime.UtcNow &&
                                                     v.UsageCount < v.UsageLimit);
            var dto = vouchers.Select(voucher =>
            {
                return new VoucherDto
                {
                    Infor =
                    {
                        VoucherId = voucher.VoucherId,
                        VoucherCode = voucher.VoucherCode,
                        DiscountPercent = voucher.DiscountPercent * 100.0f
                    },
                    Purpose = voucher.Purpose,
                    StartDate = voucher.StartDate,
                    EndDate = voucher.EndDate,
                    UsageLimit = voucher.UsageLimit,
                    UsageCount = voucher.UsageCount,
                    BeUsed = VoucherStatus(voucher)
                };
            });
            return ServiceResult<IEnumerable<VoucherDto>>.Success(dto);
        }
        public override ServiceResult<VoucherDto> GetById(int VoucherId)
        {
            Voucher voucher;
            try
            {
                voucher = _repository.GetSingleById(VoucherId);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<VoucherDto>.NotFound($"Khồng tồn tại voucher có Id = {VoucherId}");
            }
            var dto = new VoucherDto
            {
                Infor = new VoucherBasicInfor
                {
                    VoucherId = voucher.VoucherId,
                    VoucherCode = voucher.VoucherCode,
                    DiscountPercent = voucher.DiscountPercent * 100.0f
                },
                BranchId = voucher.BranchId,
                Purpose = voucher.Purpose,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                UsageCount = voucher.UsageCount,
                UsageLimit = voucher.UsageLimit,
                BeUsed = VoucherStatus(voucher)
            };
            return ServiceResult<VoucherDto>.Success(dto);
        }
        public ServiceResult<VoucherDto> GetVoucherCode(string voucherCode)
        {
            Voucher voucher;
            try
            {
                voucher = _repository.GetSingleByCondition(v => v.VoucherCode == voucherCode);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<VoucherDto>.NotFound($"Không tồn tại VoucherCode = {voucherCode}");
            }
            var dto = new VoucherDto
            {
                Infor = new VoucherBasicInfor
                {
                    VoucherCode = voucher.VoucherCode,
                    DiscountPercent = voucher.DiscountPercent * 100.0f
                },
                BeUsed = VoucherStatus(voucher)
            };
            return ServiceResult<VoucherDto>.Success(dto);
        }
        private UsedStatus VoucherStatus(Voucher voucher)
        {
            if(DateTime.UtcNow > voucher.EndDate || voucher.UsageCount >= voucher.UsageLimit)
            {
                return UsedStatus.Expired;
            }
            else if(DateTime.UtcNow < voucher.StartDate)
            {
                return UsedStatus.UpComing;
            }
            else
            {
                return UsedStatus.Active;
            }
        }
        private bool VoucherDiscountCheck(CreateVoucherDto dto)
        {
            float discount = dto.DiscountPercent / 100.0f;
            if(discount > 1.0f) return false;
            else return true;
        }
    }
}