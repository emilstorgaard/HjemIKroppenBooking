namespace HjemIKroppenBooking.Domain.Common;

/// <summary>
/// Every machine-readable error code the system can return. Codes are stable once published because the
/// frontend depends on them. Format: <c>area.snake_case_reason</c>. The Danish text for each code lives in the
/// API's ErrorCatalog, and a unit test verifies that both lists match.
/// </summary>
public static class ErrorCodes
{
    // Server and request
    public const string ServerUnexpectedError = "server.unexpected_error";
    public const string RequestRateLimited = "request.rate_limited";
    public const string RequestInvalid = "request.invalid";
    public const string RequestNotFound = "request.not_found";
    public const string RequestMethodNotAllowed = "request.method_not_allowed";
    public const string RequestUnsupportedMediaType = "request.unsupported_media_type";

    // Authentication and authorization
    public const string AuthInvalidCredentials = "auth.invalid_credentials";
    public const string AuthUnauthorized = "auth.unauthorized";
    public const string AuthForbidden = "auth.forbidden";
    public const string AuthWrongPassword = "auth.wrong_password";

    // Users
    public const string UserNotFound = "user.not_found";
    public const string UserEmailAlreadyExists = "user.email_already_exists";
    public const string UserInactive = "user.inactive";
    public const string UserCannotChangeOwnAccount = "user.cannot_change_own_account";

    // Service categories
    public const string ServiceCategoryNotFound = "service_category.not_found";
    public const string ServiceCategoryNameAlreadyExists = "service_category.name_already_exists";
    public const string ServiceCategorySlugAlreadyExists = "service_category.slug_already_exists";
    public const string ServiceCategoryInUse = "service_category.in_use";

    // Service types
    public const string ServiceTypeNotFound = "service_type.not_found";
    public const string ServiceTypeNameAlreadyExists = "service_type.name_already_exists";
    public const string ServiceTypeInactive = "service_type.inactive";
    public const string ServiceTypeInUse = "service_type.in_use";

    // Sessions
    public const string SessionNotFound = "session.not_found";
    public const string SessionFull = "session.full";
    public const string SessionAlreadyStarted = "session.already_started";
    public const string SessionAlreadyEnded = "session.already_ended";
    public const string SessionCancelled = "session.cancelled";
    public const string SessionStartInPast = "session.start_in_past";
    public const string SessionCapacityBelowActiveBookings = "session.capacity_below_active_bookings";
    public const string SessionHasActiveBookings = "session.has_active_bookings";
    public const string SessionServiceTypeMismatch = "session.service_type_mismatch";
    public const string SessionConcurrencyConflict = "session.concurrency_conflict";

    // Series
    public const string SeriesNotFound = "series.not_found";
    public const string SeriesCancelled = "series.cancelled";
    public const string SeriesNoUpcomingSessions = "series.no_upcoming_sessions";
    public const string SeriesAlreadyBooked = "series.already_booked";
    public const string SeriesNoActiveBookings = "series.no_active_bookings";

    // Bookings
    public const string BookingNotFound = "booking.not_found";
    public const string BookingAlreadyExists = "booking.already_exists";
    public const string BookingAlreadyCancelled = "booking.already_cancelled";
    public const string BookingCancellationClosed = "booking.cancellation_closed";

    // Validation
    public const string ValidationFailed = "validation.failed";
    public const string ValidationRequired = "validation.required";
    public const string ValidationMaxLength = "validation.max_length";
    public const string ValidationInvalidValue = "validation.invalid_value";
    public const string ValidationEnumInvalid = "validation.enum_invalid";
    public const string ValidationEmailInvalid = "validation.email_invalid";
    public const string ValidationSlugInvalid = "validation.slug_invalid";
    public const string ValidationPasswordTooShort = "validation.password_too_short";
    public const string ValidationPasswordTooLong = "validation.password_too_long";
    public const string ValidationPasswordUnchanged = "validation.password_unchanged";
    public const string ValidationCapacityOutOfRange = "validation.capacity_out_of_range";
    public const string ValidationDurationOutOfRange = "validation.duration_out_of_range";
    public const string ValidationEndBeforeStart = "validation.end_before_start";
    public const string ValidationDateRangeInvalid = "validation.date_range_invalid";
    public const string ValidationTooManyItems = "validation.too_many_items";
    public const string ValidationPageOutOfRange = "validation.page_out_of_range";
    public const string ValidationPageSizeOutOfRange = "validation.page_size_out_of_range";
}
