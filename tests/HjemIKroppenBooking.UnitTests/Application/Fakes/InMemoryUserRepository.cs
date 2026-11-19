using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

internal sealed class InMemoryUserRepository : IUserRepository
{
    public List<User> Users { get; } = [];

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Users.Find(u => u.Id == id));

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        Task.FromResult(Users.Find(u => string.Equals(u.Email, normalizedEmail, StringComparison.Ordinal)));

    public Task<bool> EmailExistsAsync(string normalizedEmail, Guid? excludeUserId, CancellationToken cancellationToken) =>
        Task.FromResult(Users.Exists(u => string.Equals(u.Email, normalizedEmail, StringComparison.Ordinal) && u.Id != excludeUserId));

    public void Add(User user) => Users.Add(user);
}
