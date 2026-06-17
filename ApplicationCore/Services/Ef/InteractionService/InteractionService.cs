using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models.Interactions;

namespace ApplicationCore.Services.Ef.InteractionService;

public class InteractionService(
    IInteractionRepository repo,
    IUnitOfWork uow)
    : IInteractionService
{
    public async Task<Interaction> AddAsync(Guid contactId, CreateInteractionDto dto, Guid userId)
    {
        if (dto.Date > DateTime.UtcNow.AddMinutes(1))
            throw new ArgumentException("Interaction date cannot be in the future");

        Interaction entity = dto switch
        {
            CreateEmailInteractionDto e => new EmailInteraction
            {
                Subject = e.Subject,
                FromAddress = e.FromAddress,
                ToAddress = e.ToAddress
            },
            CreateSmsInteractionDto s => new SmsInteraction
            {
                PhoneNumber = s.PhoneNumber
            },
            CreateMeetingInteractionDto m => new MeetingInteraction
            {
                Location = m.Location,
                EndTime = m.EndTime
            },
            _ => throw new ArgumentException("Unknown interaction type")
        };

        entity.Id = Guid.NewGuid();
        entity.Contact = uow.Contacts.FindById(contactId)
            ?? throw new KeyNotFoundException($"Contact {contactId} not found");
        entity.CreatedById = userId;
        entity.Date = dto.Date;
        entity.Content = dto.Content;

        await repo.AddAsync(entity);
        await uow.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<InteractionResultDto>> GetHistoryAsync(
        Guid contactId,
        DateTime? from = null,
        DateTime? to = null,
        InteractionType? type = null,
        int page = 1,
        int pageSize = 20)
    {
        IEnumerable<Interaction> interactions;

        if (from.HasValue && to.HasValue)
            interactions = await repo.GetByContactInRangeAsync(contactId, from.Value, to.Value);
        else if (type.HasValue)
            interactions = await repo.GetByContactAndTypeAsync(contactId, type.Value);
        else
            interactions = await repo.GetByContactAsync(contactId, page, pageSize);

        return interactions.Select(MapToDto);
    }

    public async Task DeleteAsync(Guid interactionId, Guid userId, bool isAdmin)
    {
        var entity = await repo.GetByIdAsync(interactionId) ?? throw new KeyNotFoundException($"Interaction {interactionId} not found");
        if (entity.CreatedById != userId && !isAdmin)
            throw new UnauthorizedAccessException("Only the creator or an admin can delete");

        await repo.RemoveAsync(interactionId);
        await uow.SaveChangesAsync();
    }

    private static InteractionResultDto MapToDto(Interaction i)
    {
        var extra = new Dictionary<string, object?>();
        switch (i)
        {
            case EmailInteraction e:
                extra["subject"] = e.Subject;
                extra["fromAddress"] = e.FromAddress;
                extra["toAddress"] = e.ToAddress;
                break;
            case SmsInteraction s:
                extra["phoneNumber"] = s.PhoneNumber;
                break;
            case MeetingInteraction m:
                extra["location"] = m.Location;
                extra["endTime"] = m.EndTime;
                break;
        }
        return new InteractionResultDto(i.Id, i.Type, i.Date, i.Content, extra);
    }
}