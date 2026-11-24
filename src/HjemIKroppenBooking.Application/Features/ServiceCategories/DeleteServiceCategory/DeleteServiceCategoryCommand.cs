using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.DeleteServiceCategory;

/// <summary>Only categories without service types can be deleted.</summary>
public sealed record DeleteServiceCategoryCommand(Guid Id) : ICommand<Unit>;
