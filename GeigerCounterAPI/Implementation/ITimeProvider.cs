using System;

namespace GeigerCounterAPI.Implementation
{
    /// <summary>
    /// Interface for retrieving the current date and time
    /// </summary>
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
