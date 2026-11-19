namespace HjemIKroppenBooking.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);

    /// <summary>
    /// A valid hash of an unknown password, created with the same work factor as real hashes. Login verifies
    /// against it when the email is unknown, so the response time does not reveal whether the account exists.
    /// </summary>
    string DummyHash { get; }
}
