using Application.Interfaces;
using Application.Services.Commons;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;
using Shared;
using System.Xml.Serialization;

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
            try
            {
                _repository.GetSingleByCondition(b => b.BranchCode == dto.BranchCode);
                return ServiceResult<BranchDto>.ValidationError($"Đã tồn tại chi nhánh {dto.BranchCode}");
            }
            catch (KeyNotFoundException) { }
            try
            {
                var branch = new Branch
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
                var result = _mapper.Map<BranchDto>(branch);
                return ServiceResult<BranchDto>.Created(result);
            }
            catch(Exception ex)
            {
                return ServiceResult<BranchDto>.InternalServerError($"Lỗi tạo chi nhanh {ex.Message}");
            }
        } 
        public override ServiceResult<IEnumerable<BranchDto>> GetAll()
        {
            var branch = _repository.GetAll(includes:["Booths", "Booths.BoothHealth", "Booths.Invoices"]);
            var dto = branch.Select(branch =>
            {
                var totalBooths = branch.Booths.Count;
                var activeBooths = branch.Booths.Count(b => b.BoothHealth?.Status == Status.ONLINE);

                return new BranchDto
                {
                    BranchId = branch.BranchId,
                    BranchCode = branch.BranchCode,
                    BranchName = branch.BranchName,
                    ManagerName = branch.ManagerName,
                    Creator = branch.Creator,
                    Address = branch.Address,
                    PhoneNumber = branch.PhoneNumber,
                    CreateAt = branch.CreatedAt,
                    TotalBooths = totalBooths,
                    ActiveBooths = activeBooths,
                    Status = new StatusExtenTion().StatusToText(branch.Status),
                    MonthlyRevenue = branch.Booths
                        .SelectMany(b => b.Invoices)
                        .Where(i => i.CreatedAt.Month == DateTime.Now.Month &&
                                    i.CreatedAt.Year == DateTime.Now.Year)
                        .Sum(i => i.Price)
                };
            });
            return ServiceResult<IEnumerable<BranchDto>>.Success(dto);
        }
        public override ServiceResult<BranchDto> GetById(int id)
        {
            try
            {
                var branch = _repository.GetSingleByCondition(b => b.BranchId == id, includes: ["Booths", "Booths.BoothHealth", "Booths.Invoices"]);
                var activeBooths = branch.Booths.Count(b => b.BoothHealth?.Status == Status.ONLINE);
                var dto = new BranchDto
                {
                    BranchCode = branch.BranchCode,
                    BranchName = branch.BranchName,
                    ManagerName = branch.ManagerName,
                    Creator = branch.Creator,
                    Address = branch.Address,
                    PhoneNumber = branch.PhoneNumber,
                    CreateAt = branch.CreatedAt,
                    TotalBooths = branch.Booths.Count,
                    ActiveBooths = activeBooths,
                    Status = new StatusExtenTion().StatusToText(branch.Status),
                    MonthlyRevenue = branch.Booths
                        .SelectMany(b => b.Invoices)
                        .Where(i => i.CreatedAt.Month == DateTime.Now.Month
                                && i.CreatedAt.Year == DateTime.Now.Year)
                        .Sum(i => i.Price)
                };
                return ServiceResult<BranchDto>.Success(dto); 
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BranchDto>.NotFound($"Không có chi nhánh có id = {id}");
            }
        }
        public override async Task<ServiceResult> Delete(int id)
        {
            var branch = _repository.GetSingleById(id);
            if(branch == null)
            {
                return ServiceResult.NotFound($"Branch {id} not found");
            }
            bool activeBooths = _boothRepository
                .GetAll(includes: ["BoothHealth"])
                .Any(b => b.BranchId == id && b.BoothHealth?.Status == Status.ONLINE);
            if (activeBooths)
            {
                return ServiceResult.ValidationError("Không thể xóa chi nhánh đang có booth hoạt động");
            }
            return await base.Delete(id);
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