using GeigerCounterAPI.Models;

namespace GeigerCounterAPI.Implementation

{
    public interface IRadiationCounter
    {
        /// <summary>
        /// Store and accumulate a particle reading
        /// </summary>
        /// <param name="reading"></param>
        void TakeReading(ParticleReading reading);

        /// <summary>
        /// Calculates the average number of particles per second recorded by this instance
        /// </summary>
        /// <returns></returns>
        RadiationSample CalcSample();

    }
}