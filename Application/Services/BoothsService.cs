using System.Reflection.Metadata.Ecma335;
using Application.DTOs.Identites.Booths;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;
using Shared;

namespace Application.Services
{
    public class BoothService : GenericService<Booths, BoothDto, CreateBoothDto, int>, IBoothService
    {
        private readonly IGenericRepository<BoothError, int> _errorRepository;
        private readonly IGenericRepository<BoothHealth, int> _healthRepository;
        private readonly IGenericRepository<BoothResources, int> _resourceRepository;
        private readonly IGenericRepository<Branch, int> _branchRepository;
        public BoothService(
            IGenericRepository<Booths, int> BoothRepository,
            IGenericRepository<BoothError, int> errorRepository,
            IGenericRepository<BoothHealth, int> healthRepository,
            IGenericRepository<BoothResources, int> resourcesRepository,
            IGenericRepository<Branch, int> branchRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
            : base(BoothRepository, mapper, unitOfWork)
        {
            _errorRepository = errorRepository;
            _healthRepository = healthRepository;
            _resourceRepository = resourcesRepository;
            _branchRepository = branchRepository;
        }
        // BUSINESS
        #region Booths
        public override async Task<ServiceResult<BoothDto>> CreateAsync(CreateBoothDto dto)
        {

            if (!ValidateBoothName(dto.BoothName).IsSuccess)
            {
                return ServiceResult<BoothDto>.ValidationError(ValidateBoothName(dto.BoothName).Message);
            }
            Branch? branch;
            try
            {
                branch = _branchRepository.GetSingleByCondition(b => b.BranchCode == dto.BranchCode);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BoothDto>.NotFound($"Không tìm thấy chi nhánh {dto.BranchCode}");
            }
            try
            {
                var booth = new Booths
                {
                    BoothIp = dto.BoothIp,
                    BoothName = dto.BoothName,
                    BranchId = branch.BranchId,
                    Brand = dto.Brand,
                    Creator = dto.Creator,
                    CreatedAt = DateTime.UtcNow
                };
                _repository.Add(booth);
                await _unitOfWork.SaveChangesAsync();
                
                var resource = new BoothResources
                {
                    BoothId = booth.BoothId,
                    PaperCount = 0,
                    PaperMax = 0,
                    RibbonCount = 0,
                    RibbonMax = 0
                };
                _resourceRepository.Add(resource);
                await _unitOfWork.SaveChangesAsync();

                var health = new BoothHealth
                {
                    BoothId = booth.BoothId,
                    Status = Status.OFFLINE,
                    LastHeartbeat = DateTime.UtcNow
                };
                _healthRepository.Add(health);
                await _unitOfWork.SaveChangesAsync();
                if(booth.BoothResources == null)
                {
                    return ServiceResult<BoothDto>.InternalServerError("Lỗi tạo BoothResources");
                }
                if(booth.BoothHealth == null)
                {
                    return ServiceResult<BoothDto>.InternalServerError("Lỗi tạo BoothHealth");
                }
                var result = new BoothDto
                {
                    BoothId = booth.BoothId,
                    BranchName = branch.BranchName,
                    BoothName = booth.BoothName,
                    BoothIp = booth.BoothIp,
                    Brand = booth.Brand,
                    Status = new StatusExtenTion().StatusToText(booth.BoothHealth.Status),
                    PaperCount = 0,
                    PaperMax = booth.BoothResources.PaperMax,
                    RibbonCount = 0,
                    RibbonMax = booth.BoothResources.RibbonMax,
                    MonthlyRevenue = 0
                };
                return ServiceResult<BoothDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<BoothDto>.InternalServerError($"Lỗi tạo booth: {ex.Message}");
            }
        }
        public override ServiceResult<IEnumerable<BoothDto>> GetAll()
        {
            var booths = _repository.GetAll(includes: ["Branch", "BoothHealth", "BoothResources", "BoothErrors", "Invoices"]);
            var dto = booths.Select(booth =>
            {
                var hasError = booth.BoothErrors.Any(e => !e.IsFixed);
                return new BoothDto
                {
                    BoothId = booth.BoothId,
                    BranchName = booth.Branch?.BranchName ?? string.Empty,
                    BoothName = booth.BoothName,
                    BoothIp = booth.BoothIp,
                    Brand = booth.Brand,
                    Status = new StatusExtenTion().StatusToText(booth.BoothHealth?.Status ?? 0),
                    PaperCount = booth.BoothResources?.PaperCount ?? 0,
                    PaperMax = booth.BoothResources?.PaperMax ?? 0,
                    RibbonCount = booth.BoothResources?.RibbonCount ?? 0,
                    RibbonMax = booth.BoothResources?.RibbonMax ?? 0,
                    MonthlyRevenue = booth.Invoices
                        .Where(i => i.CreatedAt.Month == DateTime.Now.Month &&
                                    i.CreatedAt.Year == DateTime.Now.Year)
                        .Sum(i => i.Price)
                };
            }
            );
            return ServiceResult<IEnumerable<BoothDto>>.Success(dto);
        }
        public override ServiceResult<BoothDto> GetById(int id)
        {
            try
            {
                var booth = _repository.GetSingleByCondition(b => b.BoothId == id, includes: ["Branch", "BoothHealth", "BoothResources", "Invoices"]);
                if(booth.BoothHealth == null)
                {
                    return ServiceResult<BoothDto>.ValidationError($"Không tìm thấy BoothHealth của booth có id = {id}");
                }
                var dto = new BoothDto
                {
                    BranchName = booth.Branch?.BranchName ?? string.Empty,
                    BoothName = booth.BoothName,
                    BoothIp = booth.BoothIp,
                    BoothId = booth.BoothId,
                    Brand = booth.Brand,
                    Status = new StatusExtenTion().StatusToText(booth.BoothHealth.Status),
                    PaperCount = booth.BoothResources?.PaperCount ?? 0,
                    PaperMax = booth.BoothResources?.PaperMax ?? 0,
                    RibbonCount = booth.BoothResources?.RibbonCount ?? 0,
                    RibbonMax = booth.BoothResources?.RibbonMax ?? 0,
                    MonthlyRevenue = booth.Invoices
                        .Where(i => i.CreatedAt.Month == DateTime.Now.Month &&
                                    i.CreatedAt.Year == DateTime.Now.Year)
                        .Sum(i => i.Price)
                };
                return ServiceResult<BoothDto>.Success(dto);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BoothDto>.NotFound($"Không có booth id = {id}");
            }
        }
        #endregion
        
        #region BoothError

        public async Task<ServiceResult> CreateError(int boothId, CreateBoothErrorDto dto)
        {
            var booth = _repository.GetSingleById(boothId);
            if(booth == null)
            {
                return ServiceResult<BoothErrorDto>.NotFound($"Không tìm thấy booth có id = {boothId}");
            }
            try
            {
                var error = new BoothError
                {
                    BoothId = boothId,
                    Cause = dto.Cause,
                    ErrorCode = new MessageError(dto.Cause),
                    IsFixed = false,
                    CreatedAt = DateTime.UtcNow
                };
                _errorRepository.Add(error);
                await _unitOfWork.SaveChangesAsync();

                var result = new BoothErrorDto
                {
                    ErrorCode = error.ErrorCode,
                    Cause = error.Cause,
                    Solution = error.Solution,
                    IsFixed = error.IsFixed ? "Đã xử lý" : "Chưa xử lý",
                    CreateAt = error.CreatedAt
                };

                return ServiceResult<BoothErrorDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError($"Không thể tạo lỗi: {ex.Message}");
            }
        }
        
        public ServiceResult<IEnumerable<BoothErrorDto>> GetActiveErrors(int boothId)
        {
            var errors = _errorRepository.GetMulti(e => e.Booths != null && e.Booths.BoothId == boothId && !e.IsFixed, includes: ["Booths"]);
            var dto = errors.Select(e => new BoothErrorDto
            {
                ErrorCode = new MessageError(e.ErrorCode),
                Cause = e.Cause,
                Solution = e.Solution,
                ResolvedBy = e.ResolvedBy,
                IsFixed = e.IsFixed ? "Đã xử lý" : "Chưa xử lý",
                CreateAt = e.CreatedAt
            });
            return ServiceResult<IEnumerable<BoothErrorDto>>.Success(dto);
        }
        public ServiceResult<IEnumerable<BoothErrorDto>> GetAllErrors(int boothId)
        {
            var errors = _errorRepository.GetMulti(e => e.Booths != null && e.Booths.BoothId == boothId, includes: ["Booths"]);
            var dto = errors.Select(e => new BoothErrorDto
            {
                ErrorCode = new MessageError(e.ErrorCode),
                Cause = e.Cause,
                Solution = e.Solution,
                ResolvedBy = e.ResolvedBy,
                IsFixed = e.IsFixed ? "Đã xử lý" : "Chưa xử lý",
                CreateAt = e.CreatedAt
            });
            return ServiceResult<IEnumerable<BoothErrorDto>>.Success(dto);
        }
        public async Task<ServiceResult> FixError(int boothId, string cause)
        {
            if (!_repository.CheckContains(b => b.BoothId == boothId))
                return ServiceResult.NotFound($"Không tìm thấy booth {boothId}");

            if (!_errorRepository.CheckContains(e => e.Cause == cause))
                return ServiceResult.NotFound($"Không tìm thấy lỗi: {cause}");

            try
            {
                var error = _errorRepository.GetSingleByCondition(
                    e => e.Cause == cause
                    && e.Booths != null 
                    && e.Booths.BoothId == boothId
                    && !e.IsFixed,
                    includes: ["Booths"]);
                error.IsFixed = true;
                _errorRepository.Update(error);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.NotFound($"Không tìm thấy lỗi: '{cause}' cho booth id = {boothId}");
            }
        }
        #endregion
        #region BoothResources
        public async Task<ServiceResult> UpdateResources(int boothId, int? paper, int? ribbon)
        {
            if (!_repository.CheckContains(b => b.BoothId == boothId))
                return ServiceResult.NotFound($"Không tìm thấy booth có id = {boothId}");

            try
            {
                var resource = _resourceRepository.GetSingleByCondition(
                    r => r.Booths != null && r.Booths.BoothId == boothId,
                    includes: ["Booths"]);

                if (paper != null) resource.PaperCount += paper.Value;
                if (ribbon != null) resource.RibbonCount += ribbon.Value;
                
                _resourceRepository.Update(resource);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.NotFound($"Không tìm thấy tài nguyên trong booth id = {boothId}");
            }
        }
        public async Task<ServiceResult> SetBoothStorage (int boothId, int? paperMax, int? ribbonMax)
        {
            if (!_repository.CheckContains(b => b.BoothId == boothId))
                return ServiceResult.NotFound($"Không tìm thấy booth có id = {boothId}");
            try
            {
                var resource = _resourceRepository.GetSingleByCondition(
                    r => r.Booths != null && r.Booths.BoothId == boothId,
                    includes: ["Booths"]);

                if (paperMax != null) resource.PaperMax = paperMax.Value;
                if (ribbonMax != null) resource.RibbonMax = ribbonMax.Value;
                
                _resourceRepository.Update(resource);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.NotFound($"Không tìm thấy tài nguyên trong booth id = {boothId}");
            }
        }
        #endregion
        // VALIDATION
        private ServiceResult ValidateBoothName(string? name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult.Success();
            }
            if(name.Length < 3)
            {
                return ServiceResult.ValidationError("Booth name is too short");
            }
            if(name.Length > 100)
            {
                return ServiceResult.ValidationError("Booth name is too long");
            }
            return ServiceResult.Success();
        }
    }
}