using System.Text.Json.Serialization;
using ApplicationCore.Models.Interactions;

namespace ApplicationCore.Services.Ef.InteractionService;

public interface IInteractionService
{
    Task<Interaction> AddAsync(Guid contactId, CreateInteractionDto dto, Guid userId);
    Task<IEnumerable<InteractionResultDto>> GetHistoryAsync(
        Guid contactId,
        DateTime? from = null,
        DateTime? to = null,
        InteractionType? type = null,
        int page = 1,
        int pageSize = 20);
    Task DeleteAsync(Guid interactionId, Guid userId, bool isAdmin);
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(CreateEmailInteractionDto), "Email")]
[JsonDerivedType(typeof(CreateSmsInteractionDto), "Sms")]
[JsonDerivedType(typeof(CreateMeetingInteractionDto), "Meeting")]
public abstract record CreateInteractionDto
{
    public abstract InteractionType Type { get; }
    public DateTime Date { get; init; } = DateTime.UtcNow;
    public string Content { get; init; } = string.Empty;
}

public record CreateEmailInteractionDto : CreateInteractionDto
{
    public override InteractionType Type => InteractionType.Email;
    public string? Subject { get; init; }
    public string? FromAddress { get; init; }
    public string? ToAddress { get; init; }
}

public record CreateSmsInteractionDto : CreateInteractionDto
{
    public override InteractionType Type => InteractionType.Sms;
    public string PhoneNumber { get; init; } = string.Empty;
}

public record CreateMeetingInteractionDto : CreateInteractionDto
{
    public override InteractionType Type => InteractionType.Meeting;
    public string? Location { get; init; }
    public DateTime? EndTime { get; init; }
}

public record InteractionResultDto(
    Guid Id,
    InteractionType Type,
    DateTime Date,
    string Content,
    Dictionary<string, object?> Extra
);