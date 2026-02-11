namespace InfotecsBackend.Models.Emtities;

public record Session
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Guid DeviceId { get; init; }
    
    public Device Device { get; init; }
    
    
    public string Name { get; init; } = string.Empty;
    
    public DateTime StartTime { get; init; }
    
    public DateTime EndTime { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public string Version { get; init; } = string.Empty;
    
    public bool IsDeleted { get; set; }
}