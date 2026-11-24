namespace HjemIKroppenBooking.Infrastructure.Persistence;

/// <summary>Names of unique indexes. <see cref="UnitOfWork"/> uses them to map violations to error codes.</summary>
internal static class IndexNames
{
    public const string ActiveBookingPerUser = "UX_Bookings_SessionId_UserId_Active";
    public const string UserEmail = "UX_Users_Email";
    public const string ServiceTypeName = "UX_ServiceTypes_CategoryId_Name";
    public const string ServiceCategoryName = "UX_ServiceCategories_Name";
    public const string ServiceCategorySlug = "UX_ServiceCategories_Slug";
}
