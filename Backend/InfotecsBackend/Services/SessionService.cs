using InfotecsBackend.Errors.Exceptions;
using InfotecsBackend.Models.DTO.Device;
using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.DTO.Session;
using InfotecsBackend.Models.Emtities;
using InfotecsBackend.Models.Mapping.Extentions;
using InfotecsBackend.Repositories.Interfaces;
using InfotecsBackend.Services.Interfaces;
using Semver;

namespace InfotecsBackend.Services;

public class SessionService : ISessionService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        IDeviceRepository deviceRepository,
        ISessionRepository sessionRepository,
        ILogger<SessionService> logger)
    {
        _deviceRepository = deviceRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Обработка входящей сессии от стороннего приложения
    /// </summary>
    public async Task HandleSessionAsync(SessionResponse sessionInfo, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Handling session for device: {DeviceId}, session: {SessionId}", 
                sessionInfo.DeviceId, sessionInfo.Id);
            
            if (sessionInfo.DeviceId == Guid.Empty)
                throw new ValidationException("Device ID is required");
            
            Device? device = null;
            try
            {
                device = await _deviceRepository.GetDeviceAsync(sessionInfo.DeviceId, token);
            }
            catch (DeviceNotFoundException)
            {
                _logger.LogInformation("Device not found, will create new: {DeviceId}", sessionInfo.DeviceId);
            }
            
            if (device == null)
            {
                device = new Device(sessionInfo.DeviceId);
                await _deviceRepository.AddDeviceAsync(device, token);
            }
            else
            {
                device.LastSeenAt = DateTime.UtcNow;
                await _deviceRepository.UpdateDeviceAsync(device.Id, device, token);
            }
            
            var session = sessionInfo.SessionToEntity();
            await _sessionRepository.AddSessionAsync(session, token);
            
            _logger.LogInformation("Successfully handled session {SessionId} for device {DeviceId}", 
                session.Id, sessionInfo.DeviceId);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling session for device: {DeviceId}", sessionInfo.DeviceId);
            throw new AppException($"Failed to handle session for device {sessionInfo.DeviceId}", 500);
        }
    }

    /// <summary>
    /// Получение списка всех устройств с пагинацией
    /// </summary>
    public async Task<Page<DeviceResponse>> GetAllDevicesAsync(PageFilter paginationFilter, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Getting all devices page. Offset: {Offset}, Limit: {Limit}", 
                paginationFilter.Offset, paginationFilter.elementsLimit);
            
            var devicesPage = await _deviceRepository.GetDevicesAsync(paginationFilter, token);
            
            var deviceResponses = devicesPage.Items.Select(d => d.DeviceToResponse()).ToList();
            
            return new Page<DeviceResponse>(deviceResponses, devicesPage.TotalItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices page");
            throw new AppException("Failed to get devices", 500);
        }
    }

    /// <summary>
    /// Получение сессий устройства с пагинацией
    /// </summary>
    public async Task<Page<SessionResponse>> GetDeviceSessionsAsync(
        Guid deviceId, 
        PageFilter paginationFilter, 
        CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Getting sessions for device: {DeviceId}", deviceId);
            
            var sessionsPage = await _sessionRepository.GetSessionsByDeviceIdAsync(deviceId, paginationFilter, token);
            
            var sessionResponses = sessionsPage.Items.Select(s => s.SessionToResponse()).ToList();
            
            return new Page<SessionResponse>(sessionResponses, sessionsPage.TotalItems);
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions for device: {DeviceId}", deviceId);
            throw new AppException($"Failed to get sessions for device {deviceId}", 500);
        }
    }

    /// <summary>
    /// Мягкое удаление сессий по параметрам
    /// </summary>
    public async Task DeleteSessionsAsync(SessionDelete deleteParameters, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Deleting sessions with parameters: {@DeleteParameters}", deleteParameters);
            
            if (deleteParameters == null)
                throw new ValidationException("Delete parameters cannot be null");
            
            if (!deleteParameters.DeviceId.HasValue && 
                (deleteParameters.SessionIds == null || !deleteParameters.SessionIds.Any()) && 
                !deleteParameters.ClearDate.HasValue)
            {
                throw new ValidationException("At least one delete parameter must be specified: DeviceId, SessionIds, or ClearDate");
            }
            
            if (deleteParameters.ClearDate.HasValue && deleteParameters.ClearDate.Value > DateTime.UtcNow)
            {
                throw new ValidationException("Clear date cannot be in the future");
            }
            
            await _sessionRepository.SoftDeleteSessionsAsync(deleteParameters, token);
            
            _logger.LogInformation("Successfully deleted sessions");
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (SessionNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sessions");
            throw new AppException("Failed to delete sessions", 500);
        }
    }
    
    /// <summary>
    /// Получение устройства по ID
    /// </summary>
    public async Task<DeviceResponse> GetDeviceByIdAsync(Guid deviceId, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Getting device by ID: {DeviceId}", deviceId);
            
            var device = await _deviceRepository.GetDeviceAsync(deviceId, token);
            
            return device.DeviceToResponse();
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device by ID: {DeviceId}", deviceId);
            throw new AppException($"Failed to get device {deviceId}", 500);
        }
    }
    
    /// <summary>
    /// Создание нового девайса
    /// </summary>
    public async Task<DeviceResponse> CreateDeviceAsync(CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Creating device");

            var device = new Device(Guid.NewGuid())
            {
                CreatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
            };
            await _deviceRepository.AddDeviceAsync(device, token);
            return device.DeviceToResponse();
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding device");
            throw new AppException("Failed to create device", 500);
        }
    }
}