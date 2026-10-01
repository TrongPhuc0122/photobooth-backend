using Application.Interfaces;
using Application.DTOs.Identites.Booths;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Shared.Results;
using Shared;
using Shared.QueryParameter;
using Application.DTOs.Commons;
using System.Linq.Expressions;
using Application.DTOs.Identites;

namespace Application.Services
{
    public class BoothService : GenericService<Booths, BoothDto, CreateBoothDto, Guid>, IBoothService
    {
        private readonly IGenericRepository<BoothError, int> _errorRepository;
        private readonly IGenericRepository<BoothHealth, int> _healthRepository;
        private readonly IGenericRepository<BoothResources, int> _resourceRepository;
        private readonly IGenericRepository<Branch, int> _branchRepository;
        private readonly IGenericRepository<Setting, int> _settingRepository;
        private readonly IGenericRepository<SettingHistory, int> _settingHistoryRepository;

        public BoothService(
            IGenericRepository<Booths, Guid> BoothRepository,
            IGenericRepository<BoothError, int> errorRepository,
            IGenericRepository<BoothHealth, int> healthRepository,
            IGenericRepository<BoothResources, int> resourcesRepository,
            IGenericRepository<Branch, int> branchRepository,
            IGenericRepository<Setting, int> settingRepository,
            IGenericRepository<SettingHistory, int> settingHistoryRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
            : base(BoothRepository, mapper, unitOfWork)
        {
            _errorRepository = errorRepository;
            _healthRepository = healthRepository;
            _resourceRepository = resourcesRepository;
            _branchRepository = branchRepository;
            _settingRepository = settingRepository;
            _settingHistoryRepository = settingHistoryRepository;
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
                    Infor =
                    {
                        BoothId = booth.BoothId,
                        BoothName = booth.BoothName
                    },
                    BranchId = booth.BranchId,
                    BranchName = branch.BranchName,
                    BoothIp = booth.BoothIp,
                    Brand = booth.Brand,
                    Status = booth.BoothHealth.Status,
                    PaperCount = booth.BoothResources.PaperCount,
                    PaperMax = booth.BoothResources.PaperMax,
                    RibbonCount = booth.BoothResources.RibbonCount,
                    RibbonMax = booth.BoothResources.RibbonMax,
                    MonthlyRevenue = 0m
                };
                return ServiceResult<BoothDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<BoothDto>.InternalServerError($"Lỗi tạo booth: {ex.Message}");
            }
        }
        public ServiceResult<PagedResult<BoothDto>> GetAll(BoothQueryParameters parameters)
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
                Expression<Func<Booths, bool>>? predicate = null;
                if(parameters.BranchId.HasValue || parameters.Status.HasValue)
                {
                    predicate = b =>
                        (!parameters.BranchId.HasValue || b.BranchId == parameters.BranchId.Value) &&
                        (!parameters.Status.HasValue || b.BoothHealth!.Status == parameters.Status);
                }

                string[] searchProperties = { "BoothName", "BoothIp", "Brand" };
                string[] includes = {"Branch", "BoothHealth", "BoothResources", "BoothErrors", "Invoices"};

                var pagedEntities = _repository.GetPaged(predicate, genericParams, searchProperties, includes);
                var now = DateTime.UtcNow;
                var result = pagedEntities.Items.Select(b => new BoothDto
                {
                    Infor =
                    {
                        BoothId = b.BoothId,
                        BoothName = b.BoothName
                    },
                    BranchId = b.BranchId,
                    BranchName = b.Branch.BranchName,
                    BranchCode = b.Branch.BranchCode,
                    BoothIp = b.BoothIp,
                    Brand = b.Brand,
                    Status = b.BoothHealth!.Status,
                    PaperCount = b.BoothResources!.PaperCount,
                    PaperMax = b.BoothResources!.PaperMax,
                    RibbonCount = b.BoothResources!.RibbonCount,
                    RibbonMax = b.BoothResources!.RibbonMax,
                    MonthlyRevenue = b.Invoices
                        .Where(i => i.CreatedAt.Month == DateTime.Now.Month &&
                                i.CreatedAt.Year == DateTime.Now.Year)
                        .Sum(i => i.Price)
                });
                var pagedResult = new PagedResult<BoothDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );
                return ServiceResult<PagedResult<BoothDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<BoothDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        public override ServiceResult<BoothDto> GetById(Guid id)
        {
            Booths booth;
            try
            {
                string[] includes = { "Branch", "BoothHealth", "BoothResources", "Invoices" };
                booth = _repository.GetSingleByCondition(
                    b => b.BoothId == id, 
                    includes
                );
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BoothDto>.NotFound($"Không tồn tại BoothId = {id}");
            }
            try
            {   
                var dto = new BoothDto
                {
                    Infor =
                    {
                        BoothId = booth.BoothId,
                        BoothName = booth.BoothName
                    },
                    BranchId = booth.BranchId,
                    BranchName = booth.Branch.BranchName,
                    BranchCode = booth.Branch.BranchCode,
                    BoothIp = booth.BoothIp,
                    Brand = booth.Brand,
                    Status = booth.BoothHealth!.Status,
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
            catch (Exception ex)
            {
                return ServiceResult<BoothDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }     

        public ServiceResult<IEnumerable<BoothOptionDto>> GetAllOptions()
        {
            try
            {
                string[] includes = { "Branch" };
                var booths = _repository.GetAll(includes);
                var result = booths.Select(b => new BoothOptionDto
                {
                    BoothId = b.BoothId,
                    BoothName = b.BoothName,
                    BranchCode = b.Branch.BranchCode,
                    BranchName = b.Branch.BranchName
                });
                return ServiceResult<IEnumerable<BoothOptionDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<BoothOptionDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }

        #endregion
        
        #region BoothError

        public async Task<ServiceResult> CreateError(Guid boothId, CreateBoothErrorDto dto)
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
                    IsFixed = error.IsFixed,
                    CreateAt = error.CreatedAt
                };

                return ServiceResult<BoothErrorDto>.Created(result);
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError($"Không thể tạo lỗi: {ex.Message}");
            }
        }
        
        public ServiceResult<PagedResult<BoothErrorDto>> GetActiveErrors(Guid boothId, CommonQueryParameters parameters)
        {
            try
            {
                var genericParams = new GenericQueryParameters
                {
                    Index = parameters.Index,
                    PageSize = parameters.PageSize,
                    SortBy = parameters.SortBy,
                    SortDirection = parameters.SortDirection,
                    Search = parameters.Search
                };

                string[] searchProperties = { "Booths" };
                string[] includes = [];

                var pagedEntities = _errorRepository.GetPaged(
                    predicate: e => e.Booths != null && e.Booths.BoothId == boothId && !e.IsFixed,
                    genericParams,
                    searchProperties,
                    includes
                );

                var result = pagedEntities.Items
                    .Select(e => new BoothErrorDto
                    {
                        ErrorId = e.ErrorId,
                        ErrorCode = new MessageError(e.ErrorCode),
                        Cause = e.Cause,
                        Solution = e.Solution,
                        ResolvedBy = e.ResolvedBy,
                        IsFixed = e.IsFixed,
                        CreateAt = e.CreatedAt
                    });

                var pagedResult = new PagedResult<BoothErrorDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );

                return ServiceResult<PagedResult<BoothErrorDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<BoothErrorDto>>
                    .InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        public ServiceResult<PagedResult<BoothErrorDto>> GetAllErrors(Guid boothId, CommonQueryParameters parameters)
        {
            try
            {
                var genericParams = new GenericQueryParameters
                {
                    Index = parameters.Index,
                    PageSize = parameters.PageSize,
                    SortBy = parameters.SortBy,
                    SortDirection = parameters.SortDirection,
                    Search = parameters.Search
                };
                string[] searchProperties = { "Booths", };
                string[] includes = [];
                var pagedEntities = _errorRepository.GetPaged(
                    predicate: e => e.Booths != null && e.Booths.BoothId == boothId,
                    genericParams, 
                    searchProperties, 
                    includes);
                var result = pagedEntities.Items.Select(e => new BoothErrorDto
                {
                    ErrorId = e.ErrorId,
                    ErrorCode = e.ErrorCode,
                    Cause = e.Cause,
                    Solution = e.Solution,
                    ResolvedBy = e.ResolvedBy,
                    IsFixed = e.IsFixed,
                    CreateAt = e.CreatedAt
                });

                var pagedResult = new PagedResult<BoothErrorDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );
                return ServiceResult<PagedResult<BoothErrorDto>>.Success(pagedResult);
            }      
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<BoothErrorDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }    
        }
        public async Task<ServiceResult> FixError(Guid boothId, string cause)
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
        public async Task<ServiceResult> UpdateResources(Guid boothId, int? paper, int? ribbon)
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
        public async Task<ServiceResult> SetBoothStorage (Guid boothId, int? paperMax, int? ribbonMax)
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
        
        #region BoothHealth
        public async Task<ServiceResult> GetHeartBeat (Guid boothId)
        {
            Booths booths;
            try
            {
                string[] includes = { "Branch", "BoothHealth", "BoothResources", "Invoices" };
                booths = _repository.GetSingleByCondition(
                    b => b.BoothId == boothId, 
                    includes
                );
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.NotFound($"Khồng tồn tại Booth có id = {boothId}");
            }
            try
            {
                var health = booths.BoothHealth;
                if(health == null) return ServiceResult.InternalServerError($"Không tồn tại BoothHealth cho Booth có id = {boothId}");

                health.LastHeartbeat = DateTime.UtcNow;
                health.Status = Status.ONLINE;
                _healthRepository.Update(health);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch(Exception ex)
            {
                return ServiceResult.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }
        private static readonly TimeSpan HeartbeatTimeout = TimeSpan.FromSeconds(60);

        public async Task<ServiceResult> SetStatus()
        {
            var threshold = DateTime.UtcNow - HeartbeatTimeout;
            var missingHealth = new List<Guid>();

            var booths = _repository.GetMulti(b => true, includes: ["BoothErrors", "BoothHealth"]);
            foreach (var booth in booths)
            {
                var health = booth.BoothHealth;
                if (health == null)
                {
                    missingHealth.Add(booth.BoothId);
                    continue;
                }

                var hasUnfixedError = booth.BoothErrors.Any(e => !e.IsFixed);
                var newStatus = hasUnfixedError ? Status.ERROR
                            : health.LastHeartbeat >= threshold ? Status.ONLINE
                            : Status.OFFLINE;

                if (health.Status != newStatus)
                {
                    health.Status = newStatus;
                    _healthRepository.Update(health);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            var branches = _branchRepository.GetMulti(b => true, includes: ["Booths", "Booths.BoothHealth"]);
            foreach (var branch in branches)
            {
                var newStatus = branch.Booths.Any(b => b.BoothHealth != null && b.BoothHealth.Status == Status.ONLINE)
                    ? Status.ONLINE
                    : Status.OFFLINE;

                if (branch.Status != newStatus)
                {
                    branch.Status = newStatus;
                    _branchRepository.Update(branch);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            return missingHealth.Count > 0
                ? ServiceResult.InternalServerError($"Không tồn tại BoothHealth cho booth: {string.Join(", ", missingHealth)}")
                : ServiceResult.Success();
        }
        #endregion
        
        #region BoothSetting
        public ServiceResult<BoothSettingDto> GetSetting(Guid boothId)
        {
            Booths booth;
            try
            {
                string[] includes = { "Setting" };
                booth = _repository.GetSingleByCondition(b => b.BoothId == boothId, includes);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult<BoothSettingDto>.NotFound($"Không tồn tại BoothId = {boothId}");
            }

            if (booth.Setting == null)
            {
                return ServiceResult<BoothSettingDto>.NotFound($"Booth {boothId} chưa được gán setting");
            }

            try
            {
                var dto = new BoothSettingDto
                {
                    SettingId = booth.Setting.SettingId,
                    Camera = booth.Setting.Camera,
                    Printer = booth.Setting.Printer,
                    System = booth.Setting.System,
                    UpdatedAt = booth.Setting.UpdatedAt
                };
                return ServiceResult<BoothSettingDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<BoothSettingDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ReportCurrentSetting(Guid boothId, CreateSettingHistoryDto dto)
        {
            try
            {
                _repository.GetSingleByCondition(b => b.BoothId == boothId);
            }
            catch (KeyNotFoundException)
            {
                return ServiceResult.NotFound($"Không tồn tại BoothId = {boothId}");
            }

            try
            {
                var history = new SettingHistory
                {
                    BoothId = boothId,
                    Camera = dto.Camera,
                    Printer = dto.Printer,
                    System = dto.System,
                    CalledAt = dto.CalledAt
                };
                _settingHistoryRepository.Add(history);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Created();
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError($"Lỗi lưu lịch sử setting: {ex.Message}");
            }
        }

        public ServiceResult<PagedResult<SettingHistoryDto>> GetSettingHistory(Guid boothId, CommonQueryParameters parameters)
        {
            if (!_repository.CheckContains(b => b.BoothId == boothId))
                return ServiceResult<PagedResult<SettingHistoryDto>>.NotFound($"Không tồn tại BoothId = {boothId}");

            try
            {
                var genericParams = new GenericQueryParameters
                {
                    Index = parameters.Index,
                    PageSize = parameters.PageSize,
                    SortBy = parameters.SortBy,
                    SortDirection = parameters.SortDirection,
                    Search = parameters.Search
                };

                string[] searchProperties = [];
                string[] includes = [];

                var pagedEntities = _settingHistoryRepository.GetPaged(
                    predicate: h => h.BoothId == boothId,
                    genericParams,
                    searchProperties,
                    includes
                );

                var result = pagedEntities.Items.Select(h => new SettingHistoryDto
                {
                    SettingHistoryId = h.SettingHistoryId,
                    Camera = h.Camera,
                    Printer = h.Printer,
                    System = h.System,
                    CalledAt = h.CalledAt
                });

                var pagedResult = new PagedResult<SettingHistoryDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
                );

                return ServiceResult<PagedResult<SettingHistoryDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<SettingHistoryDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
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