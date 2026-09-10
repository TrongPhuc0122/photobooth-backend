using System.Linq.Expressions;
using Application.DTOs;
using Application.DTOs.Commons;
using Application.Interfaces;
using Application.Interfaces.Commons;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Shared.QueryParameter;
using Shared.Results;

namespace Application.Services;
public class TopicService : GenericService<Topic, TopicDto, CreateTopicDto, int>, ITopicService
{
    public TopicService(
        IGenericRepository<Topic, int> repository,
        IMapper mapper,
        IUnitOfWork unitOfWork
    ) : base(repository, mapper, unitOfWork)
    {
        
    }

    public async override Task<ServiceResult<TopicDto>> CreateAsync(CreateTopicDto dto)
    {
        try
        {
            var topic = _repository.GetSingleByCondition(t => t.TopicName == dto.TopicName);
            return ServiceResult<TopicDto>.InternalServerError($"Đã tồn tại topic: {dto.TopicName}");
        }
        catch (KeyNotFoundException)
        {
            var topic = new Topic
            {
                TopicName = dto.TopicName,
                layoutType = dto.layoutType,
                BranchCode = dto.BranchCode,
                CreateAt = DateTime.UtcNow
            };

            _repository.Add(topic);
            await _unitOfWork.SaveChangesAsync();

            var result = new TopicDto
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName,
                layoutType = topic.layoutType,
                FrameCount = 0,
                BranchCode = topic.BranchCode,
                CreateAt = topic.CreateAt
            };

            return ServiceResult<TopicDto>.Created(result);
        }
        catch(Exception ex)
        {
            return ServiceResult<TopicDto>.InternalServerError($"Lỗi tạo topic: {ex.Message}");
        }

    }

    public ServiceResult<PagedResult<TopicDto>> GetAll(TopicQueryParameters parameters)
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
            Expression<Func<Topic, bool>>? predicate = null;
            if (parameters.layoutType.HasValue && !string.IsNullOrEmpty(parameters.BranchCode))
                predicate = t => t.layoutType == parameters.layoutType.Value && t.BranchCode == parameters.BranchCode;
            else if (parameters.layoutType.HasValue)
                predicate = t => t.layoutType == parameters.layoutType.Value;
            else if (!string.IsNullOrEmpty(parameters.BranchCode))
                predicate = t => t.BranchCode == parameters.BranchCode;

            string[] searchProperties = { "TopicName", "BranchCode", "layoutType" };
            string[] includes = { "Frames" };
            var pagedEntities = _repository.GetPaged(predicate, genericParams, searchProperties, includes);

            var result = pagedEntities.Items.Select(_mapper.Map<Topic, TopicDto>);

            var pagedResult = new PagedResult<TopicDto>(
                result,
                pagedEntities.TotalCount,
                pagedEntities.Index,
                pagedEntities.PageSize
            );
            return ServiceResult<PagedResult<TopicDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<TopicDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }

    public ServiceResult<IEnumerable<TopicOptionDto>> GetAllOptions()
    {
        var topics = _repository.GetAll();
        var result = topics.Select(t => new TopicOptionDto
        {
            TopicName = t.TopicName
        });
        return ServiceResult<IEnumerable<TopicOptionDto>>.Success(result);
    }
    
    public override ServiceResult<TopicDto> GetById(int id)
    {
        Topic topic;
        try
        {
            topic = _repository.GetSingleById(id);
            var result = _mapper.Map<TopicDto>(topic);
            return ServiceResult<TopicDto>.Success(result);
        }
        catch (KeyNotFoundException)
        {
            return ServiceResult<TopicDto>.NotFound($"Không tìm thấy TopicId = {id}");
        }
        catch(Exception ex)
        {
            return ServiceResult<TopicDto>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }

    }
}