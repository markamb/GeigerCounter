using System;

namespace GeigerCounterAPI.Implementation
{
    /// <summary>
    /// Concrete class for retrieving the current system date and time using the system clock
    /// </summary>
    class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
