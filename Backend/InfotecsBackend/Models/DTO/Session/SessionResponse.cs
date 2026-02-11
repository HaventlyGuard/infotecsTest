using InfotecsBackend.Models.DTO.Validations;

namespace InfotecsBackend.Models.DTO.Session;

[ValidSession]
public class SessionResponse
{
    public Guid Id { get; init; } 
    
    public Guid DeviceId { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string Version { get; init; } = string.Empty;
    
    public bool IsDeleted { get; set; }
    
    public DateTime StartTime { get; init; }
    
    public DateTime EndTime { get; init; }
}