using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.Emtities;

namespace InfotecsBackend.Repositories.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetDeviceAsync(Guid sessionInfoDeviceId, CancellationToken token);
    Task UpdateDeviceAsync(Guid sessionInfoDeviceId, Device device, CancellationToken token);
    Task AddDeviceAsync(Device device, CancellationToken token);
    Task<Page<Device>> GetDevicesAsync(PageFilter paginationFilter, CancellationToken token);
}