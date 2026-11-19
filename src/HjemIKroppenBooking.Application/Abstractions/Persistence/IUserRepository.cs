using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Loads and stores the user aggregate. Returned entities are tracked.</summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string normalizedEmail, Guid? excludeUserId, CancellationToken cancellationToken);

    void Add(User user);
}
