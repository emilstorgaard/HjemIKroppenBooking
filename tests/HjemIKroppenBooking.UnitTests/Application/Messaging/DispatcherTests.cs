using FluentValidation;
using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.UnitTests.Application.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Application.Messaging;

public sealed class DispatcherTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly PingCommandHandler _handler = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    public DispatcherTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSingleton<ICommandHandler<PingCommand, string>>(_handler);
        services.AddSingleton<IQueryHandler<PingQuery, string>, PingQueryHandler>();
        services.AddSingleton<IValidator<PingCommand>, PingCommandValidator>();
        services.AddSingleton<IValidator<PingQuery>, PingQueryValidator>();
        services.AddSingleton<IUnitOfWork>(_unitOfWork);
        services.AddSingleton<IDispatcher, Dispatcher>();
        _provider = services.BuildServiceProvider();
    }

    private IDispatcher Sut => _provider.GetRequiredService<IDispatcher>();

    [Fact]
    public async Task Send_ValidCommand_ReturnsHandlerResult()
    {
        // Arrange
        var command = new PingCommand("hello");

        // Act
        var result = await Sut.Send(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("hello", result);
        Assert.Equal(1, _handler.Calls);
    }

    [Fact]
    public async Task Send_InvalidCommand_ThrowsValidationErrorsAndSkipsHandler()
    {
        // Arrange
        var command = new PingCommand(string.Empty);

        // Act
        var exception = await Record.ExceptionAsync(() => Sut.Send(command, TestContext.Current.CancellationToken));

        // Assert
        var validation = Assert.IsType<RequestValidationException>(exception);
        var error = Assert.Single(validation.Errors);
        Assert.Equal("value", error.Field);
        Assert.Equal(ErrorCodes.ValidationRequired, error.Code);
        Assert.Equal(0, _handler.Calls);
    }

    [Fact]
    public async Task Send_ConflictOnFirstAttempt_DiscardsChangesAndRetriesOnce()
    {
        // Arrange
        _handler.Failures.Enqueue(new PersistenceConflictException(ErrorCodes.SessionConcurrencyConflict, null));

        // Act
        var result = await Sut.Send(new PingCommand("retried"), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("retried", result);
        Assert.Equal(2, _handler.Calls);
        Assert.Equal(1, _unitOfWork.DiscardCount);
    }

    [Fact]
    public async Task Send_ConflictOnBothAttempts_ThrowsConflictWithPersistenceErrorCode()
    {
        // Arrange
        _handler.Failures.Enqueue(new PersistenceConflictException(ErrorCodes.SessionConcurrencyConflict, null));
        _handler.Failures.Enqueue(new PersistenceConflictException(ErrorCodes.BookingAlreadyExists, null));

        // Act
        var exception = await Record.ExceptionAsync(() => Sut.Send(new PingCommand("x"), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.BookingAlreadyExists, ErrorType.Conflict);
        Assert.Equal(2, _handler.Calls);
    }

    [Fact]
    public async Task Send_DomainErrorOnFirstAttempt_IsNotRetried()
    {
        // Arrange
        _handler.Failures.Enqueue(DomainException.Conflict(ErrorCodes.SessionFull));

        // Act
        var exception = await Record.ExceptionAsync(() => Sut.Send(new PingCommand("x"), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.SessionFull, ErrorType.Conflict);
        Assert.Equal(1, _handler.Calls);
        Assert.Equal(0, _unitOfWork.DiscardCount);
    }

    [Fact]
    public async Task Query_InvalidQuery_ThrowsValidationErrors()
    {
        // Arrange
        var query = new PingQuery(0);

        // Act
        var exception = await Record.ExceptionAsync(() => Sut.Query(query, TestContext.Current.CancellationToken));

        // Assert
        var validation = Assert.IsType<RequestValidationException>(exception);
        Assert.Equal(ErrorCodes.ValidationPageOutOfRange, Assert.Single(validation.Errors).Code);
    }

    public void Dispose()
    {
        _provider.Dispose();
        GC.SuppressFinalize(this);
    }

    public sealed record PingCommand(string Value) : ICommand<string>;

    public sealed record PingQuery(int Page) : IQuery<string>;

    private sealed class PingCommandHandler : ICommandHandler<PingCommand, string>
    {
        public Queue<Exception> Failures { get; } = new();

        public int Calls { get; private set; }

        public Task<string> Handle(PingCommand command, CancellationToken cancellationToken)
        {
            Calls++;
            return Failures.TryDequeue(out var failure) ? Task.FromException<string>(failure) : Task.FromResult(command.Value);
        }
    }

    private sealed class PingQueryHandler : IQueryHandler<PingQuery, string>
    {
        public Task<string> Handle(PingQuery query, CancellationToken cancellationToken) => Task.FromResult("pong");
    }

    private sealed class PingCommandValidator : AppValidator<PingCommand>
    {
        public PingCommandValidator() => RuleFor(x => x.Value).NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired);
    }

    private sealed class PingQueryValidator : AppValidator<PingQuery>
    {
        public PingQueryValidator() => RuleFor(x => x.Page).GreaterThan(0).WithErrorCode(ErrorCodes.ValidationPageOutOfRange);
    }
}
