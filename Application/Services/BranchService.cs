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
        public BranchService(
            IGenericRepository<Branch, int> repository,
            IGenericRepository<Booths, int> boothRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork
        ) : base(repository, mapper, unitOfWork)
        {
            _boothRepository = boothRepository;
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
                string[] includes = {"Booths", "Booths.BoothHealth", "Booths.Invoices"};

                var pagedEntities = _repository.GetPaged(genericParams, searchProprties, includes);
                var now = DateTime.UtcNow;
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
                    MonthlyRevenue = b.Booths?
                            .SelectMany(b => b.Invoices ?? Enumerable.Empty<Invoice>())
                            .Where(i => i.CreatedAt.Month == now.Month && 
                                        i.CreatedAt.Year == now.Year && 
                                        i.FlowStatus == true)
                            .Sum(i => i.FinalPrice) ?? 0
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
                branch = _repository.GetSingleById(id);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BranchDto>.NotFound($"Không tìm thấy BranchId = {id}");
            }
            try
            {
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
                    MonthlyRevenue = branch.Booths
                        .SelectMany(b => b.Invoices)
                        .Where(i => i.CreatedAt.Month == DateTime.Now.Month
                                && i.CreatedAt.Year == DateTime.Now.Year)
                        .Sum(i => i.Price)
                };
                return ServiceResult<BranchDto>.Success(result);
            }
            catch(Exception ex)
            {
                return ServiceResult<BranchDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        public override async Task<ServiceResult> Delete(int id)
        {
            Branch branch;
            try
            {
                branch  = _repository.GetSingleById(id);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.NotFound($"Không tồn tại branch id = {id}");
            }
            bool isActive = _boothRepository.CheckContains(b => b.BranchId == id && b.BoothHealth != null && b.BoothHealth.Status == Status.ONLINE);
            if (isActive)
            {
                return ServiceResult.ValidationError("Không thể xóa chi nhánh đang có booth hoạt động");
            }
            branch.IsDeleted = true;
            _repository.Update(branch);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
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