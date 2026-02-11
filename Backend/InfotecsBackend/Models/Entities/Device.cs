namespace InfotecsBackend.Models.Emtities;

public record Device
{
    public Device(Guid id)
    {
        Id = id;
    }
    
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;

    public List<Session> Sessions { get; set; } = [];
}