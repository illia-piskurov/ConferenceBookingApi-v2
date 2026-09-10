namespace ConferenceBooking.LoadTest.Models;

public record RequestResult(
    string Method,
    string Endpoint,
    int StatusCode,
    double DurationMs,
    bool IsSuccess,
    string? ErrorMessage = null);
