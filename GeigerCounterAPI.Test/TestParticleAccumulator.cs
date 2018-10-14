using System;
using GeigerCounterAPI.Implementation;
using GeigerCounterAPI.Models;
using Moq;
using NUnit.Framework;

namespace GeigerCounterAPI.Test
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
            var t2 = t1.AddSeconds(2.0);

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2);

            var p = new ParticleAccumulator(mockClock.Object);
            p.TakeReading(new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 1} );
            p.TakeReading(new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 0 });
            p.TakeReading(new ParticleReading() { Alpha = 1, Beta = 2, Gamma = 1 });
            p.TakeReading(new ParticleReading() { Alpha = 2, Beta = 4, Gamma = 0 });
            p.TakeReading(new ParticleReading() { Alpha = 2, Beta = 4, Gamma = 1 });
            p.TakeReading(new ParticleReading() { Alpha = 2, Beta = 4, Gamma = 0 });
            var s = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(2));
            Assert.AreEqual(6, s.Samples);
            Assert.AreEqual(4.5, s.Alpha, DblDelta);
            Assert.AreEqual(9.0, s.Beta, DblDelta);
            Assert.AreEqual(1.5, s.Gamma, DblDelta);
        }

        [Test]
        public void When_ZeroCounts_Expect_CorrectPerSecondRates()
        {
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1.AddSeconds(2.0);

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2);

            var p = new ParticleAccumulator(mockClock.Object);
            p.TakeReading(new ParticleReading() { Alpha = 0, Beta = 0, Gamma = 0 });
            p.TakeReading(new ParticleReading() { Alpha = 0, Beta = 0, Gamma = 0 });
            p.TakeReading(new ParticleReading() { Alpha = 0, Beta = 0, Gamma = 0 });
            var s = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(2));
            Assert.AreEqual(3, s.Samples);
            Assert.AreEqual(0.0, s.Alpha, DblDelta);
            Assert.AreEqual(0.0, s.Beta, DblDelta);
            Assert.AreEqual(0.0, s.Gamma, DblDelta);
        }

        [Test]
        public void When_NoReadings_Expect_CorrectPerSecondRates()
        {
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1.AddSeconds(2.0);

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2);

            var p = new ParticleAccumulator(mockClock.Object);
            var s = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(2));
            Assert.AreEqual(0, s.Samples);
            Assert.AreEqual(0.0, s.Alpha, DblDelta);
            Assert.AreEqual(0.0, s.Beta, DblDelta);
            Assert.AreEqual(0.0, s.Gamma, DblDelta);
        }

        [Test]
        public void When_EqualStartAndEndTime_Expect_CorrectPerSecondRates()
        {
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1;

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2);

            var p = new ParticleAccumulator(mockClock.Object);
            p.TakeReading(new ParticleReading() { Alpha = 10, Beta = 10, Gamma = 10 });
            p.TakeReading(new ParticleReading() { Alpha = 10, Beta = 10, Gamma = 10 });
            var s = p.CalcSample();

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(2));
            Assert.AreEqual(2, s.Samples);
            Assert.AreEqual(0.0, s.Alpha, DblDelta);
            Assert.AreEqual(0.0, s.Beta, DblDelta);
            Assert.AreEqual(0.0, s.Gamma, DblDelta);
        }

        [Test]
        public void When_ReadingAfterCalc_Expect_Exception()
        {
            var t1 = new DateTime(2018, 6, 1, 12, 0, 0);
            var t2 = t1.AddSeconds(2.0);

            var mockClock = new Mock<ITimeProvider>(MockBehavior.Strict);
            mockClock.SetupSequence(x => x.Now).Returns(t1).Returns(t2);

            var reading = new ParticleReading() {Alpha = 10, Beta = 10, Gamma = 10};
            var p = new ParticleAccumulator(mockClock.Object);
            p.TakeReading(reading);
            p.TakeReading(reading);
            var s = p.CalcSample();

            Assert.Throws<InvalidOperationException>(() => p.CalcSample());
            Assert.Throws<InvalidOperationException>(() => p.TakeReading(reading));

            mockClock.Verify();
            mockClock.VerifyGet(x => x.Now, Times.Exactly(2));
            Assert.AreEqual(2, s.Samples);
            Assert.AreEqual(10.0, s.Alpha, DblDelta);
            Assert.AreEqual(10.0, s.Beta, DblDelta);
            Assert.AreEqual(10.0, s.Gamma, DblDelta);
        }


    }

}
