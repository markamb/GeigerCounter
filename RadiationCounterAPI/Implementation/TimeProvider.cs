using System;

namespace RadiationCounterAPI.Implementation
{
    /// <summary>
    /// Concrete class for retrieving the current system date using the system clock
    /// </summary>
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
