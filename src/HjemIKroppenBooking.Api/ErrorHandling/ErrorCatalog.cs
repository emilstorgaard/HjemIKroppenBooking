using System.Collections.Frozen;
using System.Globalization;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>
/// The only place with user-facing text: one Danish message per error code. A unit test verifies that every code in
/// <see cref="ErrorCodes"/> has a message and that there are no messages for unknown codes.
/// </summary>
public static class ErrorCatalog
{
    private static readonly FrozenDictionary<string, string> Messages = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        // Server and request
        [ErrorCodes.ServerUnexpectedError] = "Der opstod en uventet fejl. Prøv igen om lidt.",
        [ErrorCodes.RequestRateLimited] = "Du har sendt for mange forespørgsler. Vent et øjeblik, og prøv igen.",
        [ErrorCodes.RequestInvalid] = "Forespørgslen er ugyldig.",
        [ErrorCodes.RequestNotFound] = "Adressen findes ikke.",
        [ErrorCodes.RequestMethodNotAllowed] = "Handlingen er ikke tilladt på denne adresse.",
        [ErrorCodes.RequestUnsupportedMediaType] = "Forespørgslen har et format, som ikke understøttes.",

        // Authentication and authorization
        [ErrorCodes.AuthInvalidCredentials] = "Forkert e-mail eller adgangskode.",
        [ErrorCodes.AuthUnauthorized] = "Du skal være logget ind.",
        [ErrorCodes.AuthForbidden] = "Du har ikke adgang til denne handling.",
        [ErrorCodes.AuthWrongPassword] = "Adgangskoden er forkert.",

        // Users
        [ErrorCodes.UserNotFound] = "Brugeren findes ikke.",
        [ErrorCodes.UserEmailAlreadyExists] = "Der findes allerede en bruger med den e-mail.",
        [ErrorCodes.UserInactive] = "Brugeren er deaktiveret.",
        [ErrorCodes.UserCannotChangeOwnAccount] = "Du kan ikke ændre din egen konto her. Brug din profil i stedet.",

        // Service categories
        [ErrorCodes.ServiceCategoryNotFound] = "Kategorien findes ikke.",
        [ErrorCodes.ServiceCategoryNameAlreadyExists] = "Der findes allerede en kategori med det navn.",
        [ErrorCodes.ServiceCategorySlugAlreadyExists] = "Der findes allerede en kategori med et lignende navn. Prøv igen.",
        [ErrorCodes.ServiceCategoryInUse] = "Kategorien har ydelser og kan derfor ikke slettes. Flyt eller slet ydelserne først.",

        // Service types
        [ErrorCodes.ServiceTypeNotFound] = "Ydelsen findes ikke.",
        [ErrorCodes.ServiceTypeNameAlreadyExists] = "Der findes allerede en ydelse med det navn i denne kategori.",
        [ErrorCodes.ServiceTypeInactive] = "Ydelsen er deaktiveret. Aktivér den først.",
        [ErrorCodes.ServiceTypeInUse] = "Ydelsen bruges af tider eller forløb, så det er ikke muligt. Du kan deaktivere den i stedet.",

        // Sessions
        [ErrorCodes.SessionNotFound] = "Tiden findes ikke.",
        [ErrorCodes.SessionFull] = "Tiden er desværre fuldt booket.",
        [ErrorCodes.SessionAlreadyStarted] = "Tiden er allerede startet.",
        [ErrorCodes.SessionAlreadyEnded] = "Tiden er allerede slut.",
        [ErrorCodes.SessionCancelled] = "Tiden er aflyst.",
        [ErrorCodes.SessionStartInPast] = "Starttidspunktet skal ligge i fremtiden.",
        [ErrorCodes.SessionCapacityBelowActiveBookings] = "Antallet af pladser kan ikke være lavere end antallet af bookinger.",
        [ErrorCodes.SessionHasActiveBookings] = "Tiden har aktive bookinger. Aflys bookingerne eller tiden først.",
        [ErrorCodes.SessionServiceTypeMismatch] = "Ydelsen passer ikke til forløbet.",
        [ErrorCodes.SessionConcurrencyConflict] = "Tiden blev ændret af en anden i samme øjeblik. Prøv igen.",

        // Series
        [ErrorCodes.SeriesNotFound] = "Forløbet findes ikke.",
        [ErrorCodes.SeriesCancelled] = "Forløbet er aflyst.",
        [ErrorCodes.SeriesNoUpcomingSessions] = "Forløbet har ingen kommende tider.",
        [ErrorCodes.SeriesAlreadyBooked] = "Der er allerede booket på alle forløbets kommende tider.",
        [ErrorCodes.SeriesNoActiveBookings] = "Der er ingen kommende bookinger i forløbet.",

        // Bookings
        [ErrorCodes.BookingNotFound] = "Bookingen findes ikke.",
        [ErrorCodes.BookingAlreadyExists] = "Der er allerede en booking på denne tid.",
        [ErrorCodes.BookingAlreadyCancelled] = "Bookingen er allerede aflyst.",
        [ErrorCodes.BookingCancellationClosed] = "Tiden er startet, så bookingen kan ikke længere aflyses.",

        // Validation
        [ErrorCodes.ValidationFailed] = "Et eller flere felter er ugyldige.",
        [ErrorCodes.ValidationRequired] = "Feltet skal udfyldes.",
        [ErrorCodes.ValidationMaxLength] = "Teksten er for lang.",
        [ErrorCodes.ValidationInvalidValue] = "Værdien er ugyldig.",
        [ErrorCodes.ValidationEnumInvalid] = "Vælg en gyldig værdi.",
        [ErrorCodes.ValidationEmailInvalid] = "Indtast en gyldig e-mailadresse.",
        [ErrorCodes.ValidationSlugInvalid] = "Må kun indeholde små bogstaver (a-z), tal og bindestreger, fx dans-og-bevaegelse.",
        [ErrorCodes.ValidationPasswordTooShort] = string.Create(CultureInfo.InvariantCulture, $"Adgangskoden skal være mindst {ValidationRules.PasswordMinLength} tegn."),
        [ErrorCodes.ValidationPasswordTooLong] = "Adgangskoden er for lang.",
        [ErrorCodes.ValidationPasswordUnchanged] = "Den nye adgangskode skal være forskellig fra den nuværende.",
        [ErrorCodes.ValidationCapacityOutOfRange] = string.Create(CultureInfo.InvariantCulture, $"Antallet af pladser skal være mellem {Session.MinCapacity} og {Session.MaxCapacity}."),
        [ErrorCodes.ValidationDurationOutOfRange] = string.Create(CultureInfo.InvariantCulture, $"Varigheden skal være mellem {Session.MinDurationMinutes} og {Session.MaxDurationMinutes} minutter."),
        [ErrorCodes.ValidationEndBeforeStart] = "Sluttidspunktet skal ligge efter starttidspunktet.",
        [ErrorCodes.ValidationDateRangeInvalid] = "Slutdatoen skal ligge efter startdatoen.",
        [ErrorCodes.ValidationTooManyItems] = string.Create(CultureInfo.InvariantCulture, $"Der er angivet for mange værdier. Et forløb kan højst have {SessionSeries.MaxSessions} tider."),
        [ErrorCodes.ValidationPageOutOfRange] = "Sidenummeret skal være 1 eller højere.",
        [ErrorCodes.ValidationPageSizeOutOfRange] = string.Create(CultureInfo.InvariantCulture, $"Sidestørrelsen skal være mellem 1 og {PagingDefaults.MaxPageSize}."),
    }.ToFrozenDictionary(StringComparer.Ordinal);

    public static IReadOnlyDictionary<string, string> All => Messages;

    /// <summary>The Danish message for the code, or the generic error message for an unknown code.</summary>
    public static string GetMessage(string code) =>
        Messages.TryGetValue(code, out var message) ? message : Messages[ErrorCodes.ServerUnexpectedError];
}
