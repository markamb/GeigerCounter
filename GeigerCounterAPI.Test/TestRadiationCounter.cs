using System;
using System.Collections.Generic;
using System.Text;
using GeigerCounterAPI.Implementation;
using GeigerCounterAPI.Models;
using NUnit.Framework;
using Moq;

namespace GeigerCounterAPI.Test
{
    [TestFixture]
    class TestRadiationCounter
    {
        // Delta for floating point comparisons
        private const double DblDelta = 1e-8;

        [Test]
        public void When_TakingTwoReadings_Expect_CorrectPerSecondRates()
        {
            var reading = new ParticleReading() {Alpha = 1, Beta = 2, Gamma = 3};
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1.AddSeconds(2.0);
            var t3 = t2;
            var t4 = t3.AddSeconds(4.0);

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2).Returns(t3).Returns(t4).Returns(t4);

            var p = new RadiationCounter(mockClock.Object);
            p.TakeReading(reading);
            p.TakeReading(reading);
            var s1 = p.CalcSample();
            p.TakeReading(reading);
            p.TakeReading(reading);
            var s2 = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(5));      

            // First reading covered 2 seconds
            Assert.AreEqual(2, s1.Samples);
            Assert.AreEqual(2.0/2.0, s1.Alpha, DblDelta);
            Assert.AreEqual(4.0/2.0, s1.Beta, DblDelta);
            Assert.AreEqual(6.0/2.0, s1.Gamma, DblDelta);

            // Second reading covered 4 seconds
            Assert.AreEqual(2, s2.Samples);
            Assert.AreEqual(2.0 / 4.0, s2.Alpha, DblDelta);
            Assert.AreEqual(4.0 / 4.0, s2.Beta, DblDelta);
            Assert.AreEqual(6.0 / 4.0, s2.Gamma, DblDelta);
        }

        [Test]
        public void When_NoReadings_CorrectZeroRates()
        {
            var reading = new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 3 };
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1.AddSeconds(2.0);
            var t3 = t2;
            var t4 = t3.AddSeconds(4.0);

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2).Returns(t3).Returns(t4).Returns(t4);

            var p = new RadiationCounter(mockClock.Object);
            var s1 = p.CalcSample();
            var s2 = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(5));

            // First reading covered 2 seconds
            Assert.AreEqual(0, s1.Samples);
            Assert.AreEqual(0.0, s1.Alpha, DblDelta);
            Assert.AreEqual(0.0, s1.Beta, DblDelta);
            Assert.AreEqual(0.0, s1.Gamma, DblDelta);

            // Second reading covered 4 seconds
            Assert.AreEqual(0, s2.Samples);
            Assert.AreEqual(0.0, s2.Alpha, DblDelta);
            Assert.AreEqual(0.0, s2.Beta, DblDelta);
            Assert.AreEqual(0.0, s2.Gamma, DblDelta);

        }

        public void When_EqualStartAndEnd_ZeroRates()
        {
            var reading = new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 3 };
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1;

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2).Returns(t2);

            var p = new RadiationCounter(mockClock.Object);
            p.TakeReading(reading);
            p.TakeReading(reading);
            p.TakeReading(reading);
            var s1 = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(3));

            // First reading covered 2 seconds
            Assert.AreEqual(3, s1.Samples);
            Assert.AreEqual(0.0, s1.Alpha, DblDelta);
            Assert.AreEqual(0.0, s1.Beta, DblDelta);
            Assert.AreEqual(0.0, s1.Gamma, DblDelta);
        }

    }
}
