using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.UpdateServiceCategory;

/// <summary>Renames a category. The slug follows the new name.</summary>
public sealed record UpdateServiceCategoryCommand(Guid Id, string Name) : ICommand<AdminServiceCategoryResponse>, IServiceCategoryFields;
