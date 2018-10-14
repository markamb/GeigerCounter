using System;
using GeigerCounterAPI.Models;

namespace GeigerCounterAPI.Implementation
{
    /// <summary>
    /// Used to accumulate samples and calculate the average particle detection rate over a period of time.
    /// Accumulation started once an instance of this class is constructed, and stops after a call to CalcSamples
    /// 
    /// Once CalcSamples has been called it is an error to call TakeReading to add more samples. This ensures no double counting may occur.
    /// CalcSamples cannot be called more than once.
    ///
    /// This class is not thread safe and assumes any locking is done externally.
    /// 
    /// </summary>
    class ParticleAccumulator
    {
        private readonly ITimeProvider _timeProvider;   // Service for loading the current time
        private readonly DateTime _startTime;           // Time we started accumulating samples
        private bool _done;                             // true when CalcSamples has been called
        private int  _samples;
        private long _alpha;
        private long _beta;
        private long _gamma;

        public ParticleAccumulator(ITimeProvider timeProvider)
        {            
            _timeProvider = timeProvider;
            _startTime = _timeProvider.Now;
            _done = false;
            _samples = 0;
            _alpha = 0;
            _beta = 0;
            _gamma = 0;
        }

        /// <summary>
        /// Take a radiation reading and process it.
        /// </summary>
        /// <param name="reading">A radiation reading to be accumulated</param>
        public void TakeReading(ParticleReading reading)
        {
            if (_done)
                throw new InvalidOperationException("Attempt to take radiation reading after sample has been calculated");

            _samples++;
            _alpha += reading.Alpha ?? 0;
            _beta += reading.Beta ?? 0;
            _gamma += reading.Gamma ?? 0;
        }

        /// <summary>
        /// Calculate a radiation sample based on all the readings accumulated to date
        /// Note that this method can only be called once.
        /// 
        /// The rates returned are an average since this class was created and now.
        /// </summary>
        /// <returns></returns>
        public RadiationSample CalcSample()
        {
            if (_done)
                throw new InvalidOperationException("Attempt to CalcSamples twice");
            _done = true;

            DateTime now = _timeProvider.Now;
            var sample = new RadiationSample
            {
                LastCalc = _startTime,
                Samples = _samples,
                Alpha = 0.0,
                Beta = 0.0,
                Gamma = 0.0
            };

            double seconds = (now - _startTime).TotalSeconds;
            if (seconds > 0.0)
            {
                sample.Alpha = _alpha / seconds;
                sample.Beta =  _beta / seconds;
                sample.Gamma = _gamma / seconds;
            }

            return sample;
        }
    }
}
