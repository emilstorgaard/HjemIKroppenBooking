using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.UpdateSeries;

public sealed record UpdateSeriesCommand(Guid Id, string Title) : ICommand<AdminSeriesResponse>;
