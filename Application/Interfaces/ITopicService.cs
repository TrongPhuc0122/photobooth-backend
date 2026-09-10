using Application.DTOs;
using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces;
public interface ITopicService : IGenericService<Topic, TopicDto, CreateTopicDto, int>
{
    ServiceResult<PagedResult<TopicDto>> GetAll(TopicQueryParameters parameters);
    ServiceResult<IEnumerable<TopicOptionDto>> GetAllOptions();
}