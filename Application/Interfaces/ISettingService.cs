using Application.DTOs.Identites;
using Application.Interfaces.Commons;
using Domain.Entities;
using Shared.Results;

namespace Application.Interfaces;
public interface ISettingService : IGenericService<Setting, SettingDto, CreateSettingDto, int>
{
    Task<ServiceResult> BoothInfor(CreateSettingLogDto dto);
}