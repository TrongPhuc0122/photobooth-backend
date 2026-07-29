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
            var topic = new Topic
            {
                TopicName = dto.TopicName
            };
            _repository.Add(topic);
            await _unitOfWork.SaveChangesAsync();

            var result = new TopicDto
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName
            };
            return ServiceResult<TopicDto>.Created(result);
        }
        catch(Exception ex)
        {
            return ServiceResult<TopicDto>.InternalServerError($"Lỗi tạo topic: {ex.Message}");
        }
    }

    public ServiceResult<PagedResult<TopicDto>> GetAll(CommonQueryParameters parameters)
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
                string[] searchProprties = {"TopicName"};
                string[] includes = {};
                var pagedEntities = _repository.GetPaged(genericParams, searchProprties, includes);
            
            var result = pagedEntities.Items.Select(t => new TopicDto
            {
                TopicId = t.TopicId,
                TopicName = t.TopicName
            }).ToList();
            var pagedResult = new PagedResult<TopicDto>(
                    result,
                    pagedEntities.TotalCount,
                    pagedEntities.Index,
                    pagedEntities.PageSize
            );
            return ServiceResult<PagedResult<TopicDto>>.Success(pagedResult);
        }
        catch(Exception ex)
        {
                return ServiceResult<PagedResult<TopicDto>>.InternalServerError($"Lỗi truy vấn: {ex.Message}");
        }
    }
    public override ServiceResult<TopicDto> GetById(int id)
    {
        Topic topic;
        try
        {
            topic = _repository.GetSingleById(id);
            var result = new TopicDto
            {
                TopicId = topic.TopicId,
                TopicName = topic.TopicName
            };
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