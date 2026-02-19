using System;

namespace MedScheduler
{
    public class DoubleBookingException : Exception
    {
        public DoubleBookingException(string message) : base(message) { }
    }

    public class InvalidAppointmentTimeException : Exception
    {
        public InvalidAppointmentTimeException(string message) : base(message) { }
    }
}
