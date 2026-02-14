using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.DTO.Session;
using InfotecsBackend.Models.Mapping.Extentions;
using InfotecsBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfotecsBackend.Controllers;

/// <summary>
/// Контроллер для работы с сессиями
/// </summary>
/// <param name="sessionService">Сервис сессий</param>
[ApiController]
[Route("api/v1/sessions")]
public class SessionController(ISessionService sessionService) : ControllerBase
{
    /// <summary>
    /// Добавить информацию о сессии от стороннего приложения
    /// </summary>
    /// <param name="sessionInfo">Данные сессии</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Статус 204 No Content</returns>
    [HttpPost]
    public async Task<ActionResult> AddSession(
        [FromBody] SessionCreate sessionInfo,
        CancellationToken token)
    {
        await sessionService.HandleSessionAsync(sessionInfo.CreateToResponse(), token);
        
        return NoContent();
    }

    /// <summary>
    /// Получить все сессии устройства по его идентификатору
    /// </summary>
    /// <param name="deviceId">Идентификатор устройства</param>
    /// <param name="paginationFilter">Параметры пагинации</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Страница с сессиями устройства</returns>
    [HttpGet("device/{deviceId:guid}")]
    public async Task<ActionResult<Page<SessionResponse>>> GetDeviceSessions(
        [FromRoute] Guid deviceId,
        [FromQuery] PageFilter paginationFilter,
        CancellationToken token)
    {
        var sessions = await sessionService.GetDeviceSessionsAsync(deviceId, paginationFilter, token);
        
        return Ok(sessions);
    }

    /// <summary>
    /// Мягкое удаление сессий по параметрам
    /// </summary>
    /// <param name="deleteParameters">Параметры удаления</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Статус 204 No Content</returns>
    [HttpDelete]
    public async Task<ActionResult> DeleteSessions(
        [FromBody] SessionDelete deleteParameters,
        CancellationToken token)
    {
        await sessionService.DeleteSessionsAsync(deleteParameters, token);
        
        return NoContent();
    }
}