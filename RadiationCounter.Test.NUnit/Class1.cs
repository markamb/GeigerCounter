using System;
using NUnit.Framework;
//using RadiationCounterAPI.Implementation;
//using RadiationCounterAPI.Models;

/*
namespace RadiationCounterAPI.Test.Nunit
{
    [TestFixture]
    public class TestParticleAccumulator
    {
        // Delta for floating point comparisons
        private const double DblDelta = 1e-8;

        [Test]
        public void When_TakingReadings_Expect_CorrectPerSecondRates()
        {
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1.AddSeconds(1.0);

            var mockClock = new Mock<ITimeProvider>();
            mockClock.Setup(x => x.Now).Returns(t1);
            mockClock.Setup(x => x.Now).Returns(t2);

            var p = new ParticleAccumulator(mockClock.Object);
            p.TakeReading(new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 0 });
            p.TakeReading(new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 0 });
            p.TakeReading(new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 0 });
            var s = p.CalcSample();

            mockClock.VerifyGet(x => x.Now, Times.Exactly(2));
            Assert.AreEqual(3, s.Samples);
            Assert.AreEqual(3.0, s.Alpha, DblDelta);
            Assert.AreEqual(6.0, s.Beta, DblDelta);
            Assert.AreEqual(0.0, s.Gamma, DblDelta);
        }
    }
}

    */
