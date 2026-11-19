using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.GetSeriesById;

public sealed record GetSeriesByIdQuery(Guid Id) : IQuery<AdminSeriesResponse>;
