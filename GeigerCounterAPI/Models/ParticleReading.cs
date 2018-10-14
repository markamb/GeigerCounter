using System.ComponentModel.DataAnnotations;

namespace GeigerCounterAPI.Models
{
    /// <summary>
    /// A model storing an individual reading of Alpha, Gamma and Beta particles
    /// </summary>
    public class ParticleReading
    {
        /// <summary>
        /// A count of the number of alpha particles detected
        /// </summary>
        [Required]
        public long? Alpha { get; set; }
        /// <summary>
        /// A count of the number of beta particles detected
        /// </summary>
        [Required]
        public long? Beta { get; set; }
        /// <summary>
        /// A count of the number of gamma particles detected
        /// </summary>
        [Required]
        public long? Gamma { get; set; }
    }
}
