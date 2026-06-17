using ApplicationCore.Primitives;

namespace ApplicationCore.Models.Interactions;

public enum InteractionType { Email, Sms, Meeting }

public abstract class Interaction : EntityBase
{
    public Contact? Contact { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Content { get; set; } = string.Empty;
    public InteractionType Type { get; protected set; }
}

public class EmailInteraction : Interaction
{
    public EmailInteraction() { Type = InteractionType.Email; }
    public string? Subject { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
}

public class SmsInteraction : Interaction
{
    public SmsInteraction() { Type = InteractionType.Sms; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class MeetingInteraction : Interaction
{
    public MeetingInteraction() { Type = InteractionType.Meeting; }
    public string? Location { get; set; }
    public DateTime? EndTime { get; set; }
    public List<string> Attendees { get; set; } = new();
}