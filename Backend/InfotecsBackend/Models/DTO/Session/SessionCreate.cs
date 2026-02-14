using InfotecsBackend.Models.DTO.Validations;

namespace InfotecsBackend.Models.DTO.Session;

[ValidSession]
public class SessionCreate
{
    public Guid DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Version { get; init; } = string.Empty;
}