using InfotecsBackend.Models.DTO.Device;
using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfotecsBackend.Controllers;

/// <summary>
/// Контроллер для работы с устройствами
/// </summary>
/// <param name="sessionService">Сервис сессий</param>
[ApiController]
[Route("api/v1/devices")]
public class DeviceController(ISessionService sessionService) : ControllerBase
{
    /// <summary>
    /// Получить все устройства с пагинацией
    /// </summary>
    /// <param name="paginationFilter">Параметры пагинации</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Страница с устройствами</returns>
    [HttpGet]
    public async Task<ActionResult<Page<DeviceResponse>>> GetAllDevices(
        [FromQuery] PageFilter paginationFilter,
        CancellationToken token)
    {
        var devices = await sessionService.GetAllDevicesAsync(paginationFilter, token);
        
        return Ok(devices);
    }

    /// <summary>
    /// Получить устройство по идентификатору
    /// </summary>
    /// <param name="deviceId">Идентификатор устройства</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Информация об устройстве</returns>
    [HttpGet("{deviceId:guid}")]
    public async Task<ActionResult<DeviceResponse>> GetDeviceById(
        [FromRoute] Guid deviceId,
        CancellationToken token)
    {
        var device = await sessionService.GetDeviceByIdAsync(deviceId, token);
        
        return Ok(device);
    }
}