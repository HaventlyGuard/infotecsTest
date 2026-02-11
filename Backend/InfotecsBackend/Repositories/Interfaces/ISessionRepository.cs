using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.DTO.Session;
using InfotecsBackend.Models.Emtities;

namespace InfotecsBackend.Repositories.Interfaces;

public interface ISessionRepository
{
    Task AddSessionAsync(Session session, CancellationToken token);
    Task<Page<Session>> GetSessionsByDeviceIdAsync(Guid deviceId, PageFilter paginationFilter, CancellationToken token);
    Task SoftDeleteSessionsAsync(SessionDelete deleteParameters, CancellationToken token);
}