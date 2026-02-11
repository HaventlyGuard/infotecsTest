using InfotecsBackend.DataAccess;
using InfotecsBackend.Errors.Exceptions;
using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.DTO.Session;
using InfotecsBackend.Models.Emtities;
using InfotecsBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfotecsBackend.Repositories;

#pragma warning disable CS1591

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<SessionRepository> _logger;

    public SessionRepository(AppDbContext context, ILogger<SessionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddSessionAsync(Session session, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Adding new session for device: {DeviceId}", session.DeviceId);
            
            var deviceExists = await _context.Devices.AnyAsync(d => d.Id == session.DeviceId, token);
            if (!deviceExists)
                throw new DeviceNotFoundException(session.DeviceId);
            
            await _context.Sessions.AddAsync(session, token);
            await _context.SaveChangesAsync(token);
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding session for device: {DeviceId}", session.DeviceId);
            throw new AppException($"Failed to add session for device {session.DeviceId}", 500);
        }
    }

    public async Task<Page<Session>> GetSessionsByDeviceIdAsync(Guid deviceId, PageFilter paginationFilter, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Getting sessions for device: {DeviceId}", deviceId);
            
            var deviceExists = await _context.Devices.AnyAsync(d => d.Id == deviceId, token);
            if (!deviceExists)
                throw new DeviceNotFoundException(deviceId);
            
            var query = _context.Sessions
                .Where(s => s.DeviceId == deviceId && !s.IsDeleted)
                .AsNoTracking();

            query = paginationFilter.SortDirection == SortType.Ascending
                ? query.OrderBy(s => s.StartTime)
                : query.OrderByDescending(s => s.StartTime);

            var totalCount = await query.CountAsync(token);
            
            var items = await query
                .Skip(paginationFilter.Offset)
                .Take(paginationFilter.elementsLimit)
                .ToListAsync(token);

            return new Page<Session>(items, totalCount);
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

    public async Task SoftDeleteSessionsAsync(SessionDelete deleteParameters, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Soft deleting sessions");
            
            var query = _context.Sessions.Where(s => !s.IsDeleted);

            if (deleteParameters.DeviceId.HasValue)
            {
                var deviceExists = await _context.Devices.AnyAsync(d => d.Id == deleteParameters.DeviceId.Value, token);
                if (!deviceExists)
                    throw new DeviceNotFoundException(deleteParameters.DeviceId.Value);
                    
                query = query.Where(s => s.DeviceId == deleteParameters.DeviceId.Value);
            }

            if (deleteParameters.SessionIds != null && deleteParameters.SessionIds.Any())
            {
                query = query.Where(s => deleteParameters.SessionIds.Contains(s.Id));
                
                var existingSessions = await query.Select(s => s.Id).ToListAsync(token);
                var missingSessions = deleteParameters.SessionIds.Except(existingSessions);
                
                if (missingSessions.Any())
                    throw new SessionNotFoundException(missingSessions.First());
            }

            if (deleteParameters.ClearDate.HasValue)
            {
                query = query.Where(s => s.EndTime <= deleteParameters.ClearDate.Value);
            }

            var sessionsToDelete = await query.ToListAsync(token);
            
            foreach (var session in sessionsToDelete)
            {
                session.IsDeleted = true;
            }

            await _context.SaveChangesAsync(token);
            
            _logger.LogInformation("Soft deleted {Count} sessions", sessionsToDelete.Count);
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
            _logger.LogError(ex, "Error soft deleting sessions");
            throw new AppException("Failed to delete sessions", 500);
        }
    }
}