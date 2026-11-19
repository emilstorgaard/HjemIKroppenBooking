namespace HjemIKroppenBooking.Api.Contracts.Requests.Profile;

/// <summary>The current password confirms the deactivation.</summary>
public sealed record DeactivateMeRequest(string Password);
