using Application.DTOs.Commons;
using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Results;

namespace Application.Interfaces;

public interface IStatusCodeService : IGenericService<StatusCode, StatusCodeDto, CreateStatusCodeDto, string>
{
    ServiceResult<PagedResult<StatusCodeDto>> GetAll(StatusCodeQueryParameters parameters);
    ServiceResult<StatusCodeDto> GetSingle(string code);
}