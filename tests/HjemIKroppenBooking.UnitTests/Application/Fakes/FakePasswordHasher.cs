using HjemIKroppenBooking.Application.Abstractions;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

/// <summary>Deterministic hasher that records which hashes were verified.</summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public const string Prefix = "hashed:";

    public List<string> VerifiedHashes { get; } = [];

    public string DummyHash => "dummy-hash";

    public string Hash(string password) => Prefix + password;

    public bool Verify(string password, string hash)
    {
        VerifiedHashes.Add(hash);
        return string.Equals(hash, Prefix + password, StringComparison.Ordinal);
    }
}
