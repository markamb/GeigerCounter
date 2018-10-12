using RadiationCounterAPI.Models;

namespace RadiationCounterAPI.Implementation

{
    public interface IRadiationCounter
    {
        /// <summary>
        /// Store and accumulate a particle reading
        /// </summary>
        /// <param name="reading"></param>
        void TakeReading(ParticleReading reading);

        /// <summary>
        /// Calulates the average number of particles per second recorded by this instance
        /// </summary>
        /// <returns></returns>
        RadiationSample CalcSample();

    }
}