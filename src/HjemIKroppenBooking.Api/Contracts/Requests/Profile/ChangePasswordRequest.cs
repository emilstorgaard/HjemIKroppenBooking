namespace HjemIKroppenBooking.Api.Contracts.Requests.Profile;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
