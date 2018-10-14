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
        private readonly ITimeProvider _TimeProvider;   // Service for loading the current time
        private readonly DateTime _StartTime;           // Time we started accumulating samples
        private bool _Done;                             // true when CalcSamples has been called
        private int  _Samples;
        private long _Alpha;
        private long _Beta;
        private long _Gamma;

        public ParticleAccumulator(ITimeProvider TimeProvider)
        {            
            _TimeProvider = TimeProvider;
            _StartTime = _TimeProvider.Now;
            _Done = false;
            _Samples = 0;
            _Alpha = 0;
            _Beta = 0;
            _Gamma = 0;
        }

        /// <summary>
        /// Take a radiation reading and process it.
        /// </summary>
        /// <param name="reading">A radiation reading to be accumulated</param>
        public void TakeReading(ParticleReading reading)
        {
            if (_Done)
                throw new InvalidOperationException("Attempt to take radiation reading after sample has been calculated");

            _Samples++;
            _Alpha += reading.Alpha;
            _Beta += reading.Beta;
            _Gamma += reading.Gamma;
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
            if (_Done)
                throw new InvalidOperationException("Attempt to CalcSamples twice");
            _Done = true;

            DateTime now = _TimeProvider.Now;
            var sample = new RadiationSample
            {
                LastCalc = _StartTime,
                Samples = this._Samples,
                Alpha = 0.0,
                Beta = 0.0,
                Gamma = 0.0
            };

            double seconds = (now - _StartTime).TotalSeconds;
            if (seconds > 0.0)
            {
                sample.Alpha = (double) _Alpha / seconds;
                sample.Beta = (double) _Beta / seconds;
                sample.Gamma = (double) _Gamma / seconds;
            }

            return sample;
        }
    }
}
