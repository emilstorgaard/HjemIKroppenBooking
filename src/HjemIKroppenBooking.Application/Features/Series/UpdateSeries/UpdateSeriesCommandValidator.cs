using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Series;

namespace HjemIKroppenBooking.Application.Features.Series.UpdateSeries;

public sealed class UpdateSeriesCommandValidator : AppValidator<UpdateSeriesCommand>
{
    public UpdateSeriesCommandValidator()
    {
        RuleFor(x => x.Title).RequiredText(SessionSeries.TitleMaxLength);
    }
}
