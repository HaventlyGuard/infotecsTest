using InfotecsBackend.Models.DTO.Session;
using InfotecsBackend.Models.Emtities;

namespace InfotecsBackend.Models.Mapping.Extentions;

public static class SessionExtensions
{
    public static SessionResponse SessionToResponse(this Session device)
    {
        return new SessionResponse()
        {
            Id = device.Id,
            IsDeleted = device.IsDeleted,
            StartTime = device.StartTime,
            EndTime = device.EndTime,
            DeviceId = device.DeviceId,
        };
    }

    public static Session SessionToEntity(this SessionResponse sessionResponse)
    {
        return new Session()
        {
            Id = sessionResponse.Id == Guid.Empty ? Guid.NewGuid() : sessionResponse.Id,
            IsDeleted = false,
            StartTime = sessionResponse.StartTime,
            EndTime = sessionResponse.EndTime,
            DeviceId = sessionResponse.DeviceId,
            CreatedAt = DateTime.UtcNow,
            Version = sessionResponse.Version,
        };
    }
}