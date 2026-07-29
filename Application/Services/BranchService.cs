using Application.Interfaces;
using Application.Services.Commons;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;
using Shared;
using Application.DTOs.Commons;
using Shared.QueryParameter;

namespace Application.Services
{
    public class BranchService 
        : GenericService<Branch, BranchDto, CreateBranchDto, int>, IBranchService
    {
        private readonly IGenericRepository<Booths, int> _boothRepository;
        private readonly IGenericRepository<Invoice, int> _invoiceRepository;

        public BranchService(
            IGenericRepository<Branch, int> repository,
            IGenericRepository<Booths, int> boothRepository,
            IGenericRepository<Invoice, int> invoiceRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
        ) : base(repository, mapper, unitOfWork)
        {
            _boothRepository = boothRepository;
            _invoiceRepository = invoiceRepository;
        }

        // BUSINESS
        public async override Task<ServiceResult<BranchDto>> CreateAsync(CreateBranchDto dto)
        {
            if (!ValidateBranchPhoneNumber(dto.PhoneNumber).IsSuccess)
            {
                return ServiceResult<BranchDto>.ValidationError("Sai format số điện thoại");
            }
            Branch branch;
            try
            {
                branch = _repository.GetSingleByCondition(b => b.BranchCode == dto.BranchCode);
                return ServiceResult<BranchDto>.ValidationError($"Đã tồn tại BranchCode {dto.BranchCode}");
            }
            catch (KeyNotFoundException)
            {}
            try{
                branch = new Branch
                {
                    BranchName = dto.BranchName,
                    BranchCode = dto.BranchCode,
                    ManagerName = dto.ManagerName,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    Status = Status.OFFLINE
                };
                _repository.Add(branch);
                await _unitOfWork.SaveChangesAsync();
                var result = new BranchDto
                {
                    Infor = new BranchBasicInfor
                    {
                        Id = branch.BranchId,
                        BranchName = branch.BranchName
                    },
                    BranchCode = branch.BranchCode,
                    ManagerName = branch.ManagerName,
                    Creator = branch.Creator,
                    Address = branch.Address,
                    PhoneNumber = branch.PhoneNumber,
                    Status = branch.Status,
                    TotalBooths = 0,
                    ActiveBooths = 0,
                    MonthlyRevenue = 0,
                };
                return ServiceResult<BranchDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<BranchDto>.InternalServerError($"Lỗi tạo Branch {ex.Message}");
            }
        } 
        public ServiceResult<PagedResult<BranchDto>> GetAll(BranchQueryParameters parameters)
        {
            try
            {
                var genericParams = new GenericQueryParameters
                {
                    Take = parameters.Take,
                    Index = parameters.Index,
                    PageSize = parameters.PageSize,
                    SortBy = parameters.SortBy,
                    SortDirection = parameters.SortDirection,
                    Search = parameters.Search
                };
                string[] searchProprties = {"BranchName", "BranchCode", "Address"};
                string[] includes = {"Booths", "Booths.BoothHealth"};

                var pagedEntities = _repository.GetPaged(genericParams, searchProprties, includes);
                var now = DateTime.UtcNow;
                var branchIds = pagedEntities.Items.Select(b => b.BranchId).ToList();
                var monthInvoices = _invoiceRepository.GetMulti(
                    i => i.CreatedAt.Month == now.Month
                        && i.CreatedAt.Year == now.Year
                        && i.FlowStatus,
                    includes: ["Booth"]);
                var revenues = monthInvoices
                    .Where(i => i.Booth != null && branchIds.Contains(i.Booth.BranchId))
                    .GroupBy(i => i.Booth!.BranchId)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.FinalPrice));

                var result = pagedEntities.Items.Select(b => new BranchDto
                {
                    Infor =
                    {
                        Id = b.BranchId,
                        BranchName = b.BranchName
                    },
                    BranchCode = b.BranchCode,
                    ManagerName = b.ManagerName,
                    Creator = b.Creator,
                    Address = b.Address,
                    PhoneNumber = b.PhoneNumber,
                    Status = b.Status,
                    CreateAt = b.CreatedAt,
                    TotalBooths = b.Booths.Count(),
                    ActiveBooths = b.Booths.Count(b => b.BoothHealth != null &&
                                                    b.BoothHealth.Status == Status.ONLINE),
                    MonthlyRevenue = revenues.GetValueOrDefault(b.BranchId, 0)
                }).ToList();
                var pagedResult = new PagedResult<BranchDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );
                return ServiceResult<PagedResult<BranchDto>>.Success(pagedResult);
            }
            catch(Exception ex)
            {
                return ServiceResult<PagedResult<BranchDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }

        }
        public override ServiceResult<BranchDto> GetById(int id)
        {
            Branch branch;
            try
            {
                branch = _repository.GetSingleByCondition(
                    b => b.BranchId == id,
                    includes: ["Booths", "Booths.BoothHealth"]);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BranchDto>.NotFound($"Không tìm thấy BranchId = {id}");
            }
            try
            {
                var now = DateTime.UtcNow;
                var result = new BranchDto
                {
                    Infor =
                    {
                        Id = branch.BranchId,
                        BranchName = branch.BranchName
                    },
                    BranchCode = branch.BranchCode,
                    ManagerName = branch.ManagerName,
                    Creator = branch.Creator,
                    Address = branch.Address,
                    PhoneNumber = branch.PhoneNumber,
                    CreateAt = branch.CreatedAt,
                    TotalBooths = branch.Booths.Count(),
                    ActiveBooths = branch.Booths.Count(b => b.BoothHealth != null &&
                                                            b.BoothHealth.Status == Status.ONLINE),
                    Status = branch.Status,
                    MonthlyRevenue = _invoiceRepository.GetMulti(
                        i => i.CreatedAt.Month == now.Month
                            && i.CreatedAt.Year == now.Year
                            && i.FlowStatus,
                        includes: ["Booth"])
                        .Where(i => i.Booth != null && i.Booth.BranchId == branch.BranchId)
                        .Sum(i => i.FinalPrice)
                };
                return ServiceResult<BranchDto>.Success(result);
            }
            catch(Exception ex)
            {
                return ServiceResult<BranchDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        public ServiceResult<IEnumerable<BranchOptionDto>> GetAllOptions()
        {
            var branches = _repository.GetAll(); // không phân trang
            var result = branches.Select(b => new BranchOptionDto
            {
                BranchCode = b.BranchCode,
                BranchName = b.BranchName
            });
            return ServiceResult<IEnumerable<BranchOptionDto>>.Success(result);
        }
        // VALIDATION
        private ServiceResult ValidateBranchName(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult.ValidationError("Branch phonenumber cannot be empty");
            }
            if(name.Length < 3)
            {
                return ServiceResult.ValidationError("Branch phonenumber is too short");
            }
            if(name.Length > 100)
            {
                return ServiceResult.ValidationError("Branch phonenumber is too long");
            }
            return ServiceResult.Success();
        }
        private ServiceResult ValidateBranchPhoneNumber(string phonenumber)
        {
            if(string.IsNullOrWhiteSpace(phonenumber))
            {
                return ServiceResult.ValidationError("Branch name cannot be empty");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(phonenumber, @"^\d{9,11}$"))
            {
                return ServiceResult.ValidationError("Phone number format is invalid");
            }
            return ServiceResult.Success();
        }
        protected override ServiceResult ValidateCreatedDto(CreateBranchDto createDto)
        {
            var branchName = ValidateBranchName(createDto.BranchName);
            if(!branchName.IsSuccess) return branchName;

            var phoneNumber = ValidateBranchPhoneNumber(createDto.PhoneNumber);
            if(!phoneNumber.IsSuccess) return phoneNumber;

            return ServiceResult.Success();
        }
    }
}