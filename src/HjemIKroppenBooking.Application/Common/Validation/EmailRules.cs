using System.Net.Mail;

namespace HjemIKroppenBooking.Application.Common.Validation;

public static class EmailRules
{
    /// <summary>A pragmatic email check: parsable, no display name, and a dot in the domain (rejects "a@mail").</summary>
    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var trimmed = email.Trim();
        if (!MailAddress.TryCreate(trimmed, out var address) || !string.Equals(address.Address, trimmed, StringComparison.Ordinal))
        {
            return false;
        }

        var host = address.Host;
        return host.Contains('.', StringComparison.Ordinal) && !host.StartsWith('.') && !host.EndsWith('.');
    }
}
