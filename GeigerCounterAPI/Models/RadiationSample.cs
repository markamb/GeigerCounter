using System;
using System.Runtime.Serialization;

namespace GeigerCounterAPI.Models
{
    /// <summary>
    /// Model for storing an average radiation sample
    /// </summary>
    public class RadiationSample
    {
        // Primary key for storing in DB
        [IgnoreDataMember]
        public long Id { get; set; }
        // Time of last calculation.
        public DateTime LastCalc { get; set; }
        // Number of samples recieved since last calulation
        public int Samples { get; set; }
        // Average number of Alpha particles per second since last calulation
        public double Alpha { get; set; }
        // Average number of Beta particles per second since last calulation
        public double Beta { get; set; }
        // Average number of Gamma particles per second since last calulation
        public double Gamma { get; set; }
    }
}
