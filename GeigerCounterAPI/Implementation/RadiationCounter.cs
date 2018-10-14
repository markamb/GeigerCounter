using System;
using GeigerCounterAPI.Models;

namespace GeigerCounterAPI.Implementation
{
    /// <summary>
    /// A singleton class for counting the number of particles and tracking average number per second
    /// </summary>
    public class RadiationCounter : IRadiationCounter
    {
        private readonly Object _lock;
        private ParticleAccumulator _accumulator;
        private readonly ITimeProvider _clock;

        public RadiationCounter(ITimeProvider clock)
        {
            _lock = new Object();
            _accumulator = new ParticleAccumulator(clock);
            _clock = clock;
        }

        /// <summary>
        /// Store and accumulate a particle reading
        /// </summary>
        /// <param name="reading"></param>
        public void TakeReading(ParticleReading reading)
        {
            lock (_lock)
            {
                _accumulator.TakeReading(reading);
            }
        }

        /// <summary>
        /// Calculates the average number of particles per second since the last call to this method
        /// </summary>
        /// <returns></returns>
        public RadiationSample CalcSample()
        {
            ParticleAccumulator accumulator;
            lock (_lock)
            {
                accumulator = _accumulator;
                _accumulator = new ParticleAccumulator(_clock);
            }

            // Now calculate the particle counts per second since the last sample was taken
            return accumulator.CalcSample();
        }
    }
}
