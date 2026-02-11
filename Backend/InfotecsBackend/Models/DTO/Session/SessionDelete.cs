namespace InfotecsBackend.Models.DTO.Session;

public class SessionDelete
{
    public DateTime? ClearDate { get; set; }
  
    public Guid? DeviceId { get; set; }

    public IEnumerable<Guid>? SessionIds { get; set; }
}