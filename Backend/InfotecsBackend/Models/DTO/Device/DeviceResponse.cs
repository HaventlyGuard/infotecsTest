namespace InfotecsBackend.Models.DTO.Device;

public class DeviceResponse
{
    public Guid Id { get; init; } 
    public DateTime CreatedAt { get; init; } 
    public DateTime LastSeenAt { get; set; } 
}