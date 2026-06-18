using System.Text.Json;
using ApplicationCore.Interfaces.UnitOfWork;
using ApplicationCore.Models;
using Microsoft.Extensions.Options;

namespace ApplicationCore.Services.DeduplicationStrategy;

public class DeduplicationStrategyService(
    IUnitOfWork uow) : IDeduplicationStrategyService
{
    public async Task<DeduplicationReportDto> FindDuplicatesAsync(DeduplicationConfigDto config)
    {
        var contacts = uow.Contacts.GetAll().ToList();
        var pairs = await FindPairsAsync(contacts, config);

        return new DeduplicationReportDto(
            contacts.Count, pairs, 0);
    }

    public async Task<DeduplicationReportDto> RemoveDuplicatesAsync(
        DeduplicationConfigDto config, Guid userId)
    {
        var contacts = uow.Contacts.GetAll().ToList();
        var pairs = await FindPairsAsync(contacts, config);

        var toRemove = new HashSet<Guid>();
        foreach (var pair in pairs)
        {
            // Keep the one with the lower ID (older), remove the newer
            toRemove.Add(pair.Id2);
        }

        int removed = 0;
        foreach (var id in toRemove)
        {
            var contact = uow.Contacts.FindById(id);
            if (contact is null) continue;

            // Save snapshot
            var snapshot = JsonSerializer.Serialize(contact);
            var removedContact = new RemovedContact
            {
                Id = Guid.NewGuid(),
                OriginalContactId = contact.Id,
                Type = contact.GetType().Name,
                JsonSnapshot = snapshot,
                RemovedAt = DateTime.UtcNow,
                RemovedById = userId.ToString(),
                Reason = $"Duplicate of {contact.Id} (score-based)"
            };
            uow.RemovedContacts.Add(removedContact);

            uow.Contacts.RemoveById(id);
            removed++;
        }

        await uow.SaveChangesAsync();

        return new DeduplicationReportDto(contacts.Count, pairs, removed);
    }

    private static async Task<List<DuplicatePairDto>> FindPairsAsync(
        List<Contact> contacts, DeduplicationConfigDto config)
    {
        IDeduplicationStrategy strategy = config.Strategy switch
        {
            DeduplicationStrategyOptions.Exact => new ExactStrategy(),
            DeduplicationStrategyOptions.Fuzzy => new FuzzyStrategy(),
            _ => new FuzzyStrategy()
        };

        var pairs = new List<DuplicatePairDto>();

        for (int i = 0; i < contacts.Count; i++)
        {
            for (int j = i + 1; j < contacts.Count; j++)
            {
                if (strategy.IsMatch(contacts[i], contacts[j], config))
                {
                    var scores = strategy.GetScores(contacts[i], contacts[j], config);
                    pairs.Add(new DuplicatePairDto(
                        contacts[i].Id, contacts[j].Id,
                        contacts[i].Name, contacts[j].Name,
                        scores.Values.Average(), scores));
                }
            }
        }

        return pairs;
    }

    public async Task<bool> IsDuplicateOfExistingAsync(Contact newContact, DeduplicationConfigDto config)
    {
        var existing = uow.Contacts.GetAll().ToList();
        IDeduplicationStrategy strategy = config.Strategy switch
        {
            DeduplicationStrategyOptions.Exact => new ExactStrategy(),
            DeduplicationStrategyOptions.Fuzzy => new FuzzyStrategy(),
            _ => new FuzzyStrategy()
        };

        foreach (var c in existing)
        {
            if (c.Id == newContact.Id) continue;
            if (strategy.IsMatch(newContact, c, config)) return true;
        }
        return false;
    }
}