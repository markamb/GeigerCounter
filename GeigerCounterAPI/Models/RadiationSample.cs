﻿using System;
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
        /// <summary>
        /// Start time of this average radiation sample
        /// </summary>
        public DateTimeOffset LastCalc { get; set; }
        /// <summary>
        /// Number of samples making up this average
        /// </summary>
        // Number of samples received since last calculation
        public int Samples { get; set; }
        /// <summary>
        /// Average number of alpha particles per second in this reading
        /// </summary>
        // Average number of Alpha particles per second since last calculation
        public double Alpha { get; set; }
        /// <summary>
        /// Average number of beta particles per second since last calculation
        /// </summary>
        // Average number of Beta particles per second since last calculation
        public double Beta { get; set; }
        /// <summary>
        /// Average number of gamma particles per second since last calculation
        /// </summary>
        // Average number of Gamma particles per second since last calculation
        public double Gamma { get; set; }
    }
}
