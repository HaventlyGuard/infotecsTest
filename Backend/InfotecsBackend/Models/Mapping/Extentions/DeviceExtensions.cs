using InfotecsBackend.Models.DTO.Device;
using InfotecsBackend.Models.Emtities;

namespace InfotecsBackend.Models.Mapping.Extentions;

public static class DeviceExtensions
{
    public static DeviceResponse DeviceToResponse(this Device device)
    {
        return new DeviceResponse()
        {
            Id = device.Id,
            CreatedAt = device.CreatedAt,
            LastSeenAt = device.LastSeenAt,
        };
    }

    public static Device DeviceToEntity(this DeviceResponse device)
    {
        return new Device(device.Id)
        {
            CreatedAt = device.CreatedAt,
            LastSeenAt = device.LastSeenAt,
        };
    }
}