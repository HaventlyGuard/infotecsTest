using InfotecsBackend.DataAccess;
using InfotecsBackend.Errors.Exceptions;
using InfotecsBackend.Models.DTO.Pagination;
using InfotecsBackend.Models.Emtities;
using InfotecsBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfotecsBackend.Repositories;

#pragma warning disable CS1591

public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeviceRepository> _logger;

    public DeviceRepository(AppDbContext context, ILogger<DeviceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Device?> GetDeviceAsync(Guid deviceId, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Getting device with ID: {DeviceId}", deviceId);
            
            var device = await _context.Devices
                .Include(d => d.Sessions)
                .FirstOrDefaultAsync(d => d.Id == deviceId, token);
            
            if (device == null)
                throw new DeviceNotFoundException(deviceId);
            
            return device;
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device with ID: {DeviceId}", deviceId);
            throw new AppException($"Failed to get device {deviceId}", 500);
        }
    }

    public async Task UpdateDeviceAsync(Guid deviceId, Device device, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Updating device with ID: {DeviceId}", deviceId);
            
            var existingDevice = await _context.Devices.FindAsync([deviceId], token);
            if (existingDevice == null)
                throw new DeviceNotFoundException(deviceId);
            
            existingDevice.LastSeenAt = device.LastSeenAt;
            
            _context.Devices.Update(existingDevice);
            await _context.SaveChangesAsync(token);
        }
        catch (DeviceNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating device with ID: {DeviceId}", deviceId);
            throw new AppException($"Failed to update device {deviceId}", 500);
        }
    }

    public async Task AddDeviceAsync(Device device, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Adding new device with ID: {DeviceId}", device.Id);
            
            var existingDevice = await _context.Devices.FindAsync([device.Id], token);
            if (existingDevice != null)
            {
                existingDevice.LastSeenAt = device.LastSeenAt;
                _context.Devices.Update(existingDevice);
            }
            else
            {
                await _context.Devices.AddAsync(device, token);
            }
            
            await _context.SaveChangesAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding device with ID: {DeviceId}", device.Id);
            throw new AppException($"Failed to add device {device.Id}", 500);
        }
    }

    public async Task<Page<Device>> GetDevicesAsync(PageFilter paginationFilter, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Getting devices page. Offset: {Offset}, Limit: {Limit}", 
                paginationFilter.Offset, paginationFilter.elementsLimit);
            
            var query = _context.Devices
                .Include(d => d.Sessions)
                .AsNoTracking();

            query = paginationFilter.SortDirection == SortType.Ascending
                ? query.OrderBy(d => d.CreatedAt)
                : query.OrderByDescending(d => d.CreatedAt);

            var totalCount = await query.CountAsync(token);
            
            var items = await query
                .Skip(paginationFilter.Offset)
                .Take(paginationFilter.elementsLimit)
                .ToListAsync(token);

            return new Page<Device>(items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices page");
            throw new AppException("Failed to get devices", 500);
        }
    }
}