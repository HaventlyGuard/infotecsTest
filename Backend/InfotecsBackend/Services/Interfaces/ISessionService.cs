using InfotecsBackend.Models.DTO.Device;
using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.DTO.Session;

namespace InfotecsBackend.Services.Interfaces;

public interface ISessionService
{
    /// <summary>
    /// Обработка входящей сессии от стороннего приложения
    /// </summary>
    Task HandleSessionAsync(SessionResponse sessionInfo, CancellationToken token);
    
    /// <summary>
    /// Получение всех устройств с пагинацией
    /// </summary>
    Task<Page<DeviceResponse>> GetAllDevicesAsync(PageFilter paginationFilter, CancellationToken token);
    
    /// <summary>
    /// Получение сессий конкретного устройства с пагинацией
    /// </summary>
    Task<Page<SessionResponse>> GetDeviceSessionsAsync(Guid deviceId, PageFilter paginationFilter, CancellationToken token);
    
    /// <summary>
    /// Мягкое удаление сессий по параметрам
    /// </summary>
    Task DeleteSessionsAsync(SessionDelete deleteParameters, CancellationToken token);
    
    /// <summary>
    /// Получение устройства по ID
    /// </summary>
    Task<DeviceResponse> GetDeviceByIdAsync(Guid deviceId, CancellationToken token);
/// <summary>
/// Создание нового девайса
/// </summary>
    Task<DeviceResponse> CreateDeviceAsync(CancellationToken token);
}