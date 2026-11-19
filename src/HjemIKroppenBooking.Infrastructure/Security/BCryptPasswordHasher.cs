using System.Globalization;
using HjemIKroppenBooking.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace HjemIKroppenBooking.Infrastructure.Security;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    private readonly int _workFactor;
    private readonly Lazy<string> _dummyHash;

    public BCryptPasswordHasher(IOptions<PasswordHashingOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _workFactor = options.Value.WorkFactor;

        // Same work factor as real hashes, so verifying against it takes as long as a real login.
        _dummyHash = new Lazy<string>(() => BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture), _workFactor));
    }

    public string DummyHash => _dummyHash.Value;

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, _workFactor);

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
