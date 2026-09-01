namespace ConferenceBooking.Bll.Common.Shared.Exceptions;

public class InvalidBookingTimeException : Exception
{
    public InvalidBookingTimeException(string message) : base(message)
    {
    }
}
