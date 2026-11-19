using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.GetPublicSeriesById;

public sealed record GetPublicSeriesByIdQuery(Guid Id) : IQuery<SeriesResponse>;
