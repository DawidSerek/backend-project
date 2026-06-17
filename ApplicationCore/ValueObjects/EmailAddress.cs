using System.Net.Mail;

namespace ApplicationCore.ValueObjects;

public record EmailAddress
{
    public string Value { get; init; }
    public string LocalPart { get; }
    public string Domain { get; }

    public EmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        var normalized = email.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalized, out var mailAddress) || mailAddress is null)
            throw new ArgumentException($"'{email}' is not a valid email address", nameof(email));

        Value = normalized;
        LocalPart = mailAddress.User;
        Domain = mailAddress.Host;
    }
}
