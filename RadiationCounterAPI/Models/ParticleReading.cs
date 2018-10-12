namespace RadiationCounterAPI.Models
{
    /// <summary>
    /// A model storing an individual reading of Alpha, Gamma and Beta particles
    /// </summary>
    public class ParticleReading
    {
        public long Alpha { get; set; }
        public long Beta { get; set; }
        public long Gamma { get; set; }
    }
}
