using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using RadiationCounterAPI.Models;
using RadiationCounterAPI.Implementation;
using Microsoft.EntityFrameworkCore;
using RadiationCounterAPI.Controllers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RadiationCounterAPI.Test
{
    [TestFixture]
    public class TestRadiationController
    {
        private Mock<RadiationCounterContext>   _MockContext;
        private Mock<IRadiationCounter>         _MockCounter;

        [SetUp]
        public void Setup()
        {
            _MockContext = new Mock<RadiationCounterContext>(MockBehavior.Strict, new DbContextOptions<RadiationCounterContext>());
            _MockCounter = new Mock<IRadiationCounter>(MockBehavior.Strict);
        }

        [TearDown]
        public void Teardown()
        {
            _MockContext = null;
            _MockCounter = null;
        }

        [Test]
        public void When_TakingReadings_Expect_CounterCalled()
        {
            // Test Data
            var reading1 = new ParticleReading { Alpha = 10, Beta = 20, Gamma = 30 };
            var reading2 = new ParticleReading { Alpha = 50, Beta = 80, Gamma = 0 };

            // Setup
            _MockCounter.Setup(x => x.TakeReading(reading1));
            _MockCounter.Setup(x => x.TakeReading(reading2));

            // Create the controller and take some readings
            var controller = new RadiationController(_MockContext.Object, _MockCounter.Object);
            controller.TakeReading(reading1);
            controller.TakeReading(reading2);

            // Validate
            _MockCounter.VerifyAll();
            _MockContext.VerifyAll();
        }

        [Test]
        public void When_TakingSample_Expect_SampleCalculatedThenStored()
        {
            // Test Data
            DateTime dt = new DateTime(2018, 10, 10, 12, 0, 0);
            var samples = new List<RadiationSample>
            {
                new RadiationSample { LastCalc = dt, Alpha = 12.0, Beta = 18.0, Gamma = 3.5, Samples = 12, Id = 1},
                new RadiationSample { LastCalc = dt.AddSeconds(30), Alpha = 1.0, Beta = 8.0, Gamma = 5, Samples = 2, Id = 2},
            };

            // Expect sample to be calculated from counter, then stored in the context DB
            _MockCounter.SetupSequence(x => x.CalcSample()).Returns(samples[0]).Returns(samples[1]);
            _MockContext.Setup(x => x.AddSampleAsync(samples[0]));
            _MockContext.Setup(x => x.SaveChangesAsync(new CancellationToken())).Returns(Task.FromResult(0));
            _MockContext.Setup(x => x.AddSampleAsync(samples[1]));
            _MockContext.Setup(x => x.SaveChangesAsync(new CancellationToken())).Returns(Task.FromResult(0));

            // now create the controller and test
            var controller = new RadiationController(_MockContext.Object, _MockCounter.Object);
            Assert.AreEqual(samples[0], controller.GetSample().Result.Value);
            Assert.AreEqual(samples[1], controller.GetSample().Result.Value);

            // Validate
            _MockCounter.VerifyAll();
            _MockContext.VerifyAll();
        }

        [Test]
        public void When_GetAllSamples_Expect_AllReturned()
        {
            // Test data
            DateTime dt = new DateTime(2018, 10, 10, 12, 0, 0);
            var samples1 = new List<RadiationSample>
            {
                new RadiationSample { LastCalc = dt, Alpha = 12.0, Beta = 18.0, Gamma = 3.5, Samples = 12, Id = 1},
                new RadiationSample { LastCalc = dt.AddSeconds(30), Alpha = 1.0, Beta = 8.0, Gamma = 5, Samples = 2, Id = 2}
            };
            var samples2 = new List<RadiationSample>(samples1);
            samples2.Add(new RadiationSample { LastCalc = dt.AddSeconds(40), Alpha = 6.0, Beta = 9.0, Gamma = 10, Samples = 10, Id = 3 });

            // Expect to calls to calc samples
            _MockContext.SetupSequence(x => x.GetSamplesListAsync()).Returns(Task.FromResult(samples1)).Returns(Task.FromResult(samples2));

            // now create the controller and test
            var controller = new RadiationController(_MockContext.Object, _MockCounter.Object);
            Assert.AreEqual(samples1, controller.GetAllSamples().Result.Value);
            Assert.AreEqual(samples2, controller.GetAllSamples().Result.Value);

            _MockCounter.VerifyAll();
            _MockContext.VerifyAll();
        }


    }
}
