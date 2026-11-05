using System.Linq.Expressions;
using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Shared;
using Shared.Results;

namespace Application.Services;

public class StatusCodeService : GenericService<StatusCode, StatusCodeDto, CreateStatusCodeDto, string>, IStatusCodeService
{
    public StatusCodeService(
        IGenericRepository<StatusCode, string> repository,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        
    }

    public override async Task<ServiceResult<StatusCodeDto>> CreateAsync(CreateStatusCodeDto dto)
    {
        if(_repository.CheckContains(c => c.Code == dto.Code))
            return ServiceResult<StatusCodeDto>.InternalServerError($"Đã tồn tại mã lỗi ${dto.Code}");


        try
        {
            var code = new StatusCode
            {
                Code = dto.Code,
                Message = dto.Message,
                CreateAt = DateTime.UtcNow,
                Type = dto.Code[0] == 'E' ? StatusType.Error : StatusType.Warning,
                //UpdateAt = null
            };
            _repository.Add(code);
            await _unitOfWork.SaveChangesAsync();

            var result = new StatusCodeDto
            {
                Code = code.Code,
                Message = code.Message,
                Type = code.Type,
                CreateAt = code.CreateAt,
                UpdateAt = code.UpdateAt
            };
            return ServiceResult<StatusCodeDto>.Success(result);
        }
        catch(Exception ex)
        {
            return ServiceResult<StatusCodeDto>.InternalServerError($"Lỗi tạo code: {ex.Message}");
        }
    }

    public ServiceResult<PagedResult<StatusCodeDto>> GetAll(StatusCodeQueryParameters parameters)
    {
        try
        {
            var genericParams = parameters.ToGenericQueryParameters();
            string[] searchProperties = { "Type", "Code"};
            string[] includes = { };

            Expression<Func<StatusCode, bool>>? predicate = null;
            if (parameters.Type.HasValue)
            {
                predicate = c => c.Type == parameters.Type.Value;
            }
            var pagedEntities = _repository.GetPaged(predicate, genericParams, searchProperties, includes);
            var result = pagedEntities.Items.Select(code => new StatusCodeDto
            {
                Code = code.Code,
                Message = code.Message,
                Type = code.Type,
                CreateAt = code.CreateAt,
                UpdateAt = code.UpdateAt
            });

            var pagedResult = new PagedResult<StatusCodeDto>(
                result,
                pagedEntities.TotalCount,
                pagedEntities.Index,
                pagedEntities.PageSize
            );
            return ServiceResult<PagedResult<StatusCodeDto>>.Success(pagedResult);
        }
        catch(Exception ex)
        {
            return ServiceResult<PagedResult<StatusCodeDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }

    public ServiceResult<StatusCodeDto> GetSingle(string code)
    {
        try
        {
            var result = _repository.GetSingleByCondition(c => c.Code == code);

            var dto = new StatusCodeDto
            {
                Code = result.Code,
                Message = result.Message,
                Type = result.Type,
                CreateAt = result.CreateAt,
                UpdateAt = result.UpdateAt
            };
            return ServiceResult<StatusCodeDto>.Success(dto);
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult<StatusCodeDto>.NotFound($"Không tồn tại ${code}");
        }
        catch(Exception ex)
        {
            return ServiceResult<StatusCodeDto>.InternalServerError($"Lỗi truy vấn ${ex}");
        }
    }

    
}