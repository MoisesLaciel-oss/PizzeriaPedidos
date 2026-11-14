using Pizzeria.Application.Dtos;

namespace Pizzeria.Application.Core
{
    public interface IBaseService<TDtoAdd, TDtoUpdate>
        where TDtoAdd : DtoBase
        where TDtoUpdate : DtoBase
    {
        Task<ServiceResult> GetAllAsync();
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> SaveAsync(TDtoAdd dto);
        Task<ServiceResult> UpdateAsync(TDtoUpdate dto);
        Task<ServiceResult> RemoveAsync(int id);
    }
}
