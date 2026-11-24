using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.CreateServiceCategory;

/// <summary>Creates a category. The slug is made from the name.</summary>
public sealed record CreateServiceCategoryCommand(string Name) : ICommand<AdminServiceCategoryResponse>, IServiceCategoryFields;
