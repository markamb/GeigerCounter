using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using GeigerCounterAPI.Models;
using GeigerCounterAPI.Implementation;
using GeigerCounterAPI.Controllers;

namespace GeigerCounterAPI.Test
{
    [TestFixture]
    public class TestGeigerCounterController
    {
        private Mock<GeigerCounterContext>   _mockContext;
        private Mock<IRadiationCounter>      _mockCounter;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<GeigerCounterContext>(MockBehavior.Strict, new DbContextOptions<GeigerCounterContext>());
            _mockCounter = new Mock<IRadiationCounter>(MockBehavior.Strict);
        }

        [TearDown]
        public void Teardown()
        {
            _mockContext = null;
            _mockCounter = null;
        }

        [Test]
        public void When_TakingReadings_Expect_CounterCalled()
        {
            // Test Data
            var reading1 = new ParticleReading { Alpha = 10, Beta = 20, Gamma = 30 };
            var reading2 = new ParticleReading { Alpha = 50, Beta = 80, Gamma = 0 };

            // Setup
            _mockCounter.Setup(x => x.TakeReading(reading1));
            _mockCounter.Setup(x => x.TakeReading(reading2));

            // Create the controller and take some readings
            var controller = new GeigerCounterController(_mockContext.Object, _mockCounter.Object);
            controller.TakeReading(reading1);
            controller.TakeReading(reading2);

            // Validate
            _mockCounter.VerifyAll();
            _mockContext.VerifyAll();
        }

        [Test]
        public void When_TakingSample_Expect_SampleCalculatedThenStored()
        {
            // Test Data
            DateTimeOffset dt = new DateTime(2018, 10, 10, 12, 0, 0);
            var samples = new List<RadiationSample>
            {
                new RadiationSample { LastCalc = dt, Alpha = 12.0, Beta = 18.0, Gamma = 3.5, Samples = 12, Id = 1},
                new RadiationSample { LastCalc = dt.AddSeconds(30), Alpha = 1.0, Beta = 8.0, Gamma = 5, Samples = 2, Id = 2}
            };

            // Expect sample to be calculated from counter, then stored in the context DB
            _mockCounter.SetupSequence(x => x.CalcSample()).Returns(samples[0]).Returns(samples[1]);
            _mockContext.Setup(x => x.AddSampleAsync(samples[0])).Returns(Task.CompletedTask);
            _mockContext.Setup(x => x.SaveChangesAsync(new CancellationToken())).Returns(Task.FromResult(0));
            _mockContext.Setup(x => x.AddSampleAsync(samples[1])).Returns(Task.CompletedTask);
            _mockContext.Setup(x => x.SaveChangesAsync(new CancellationToken())).Returns(Task.FromResult(0));

            // now create the controller and test
            var controller = new GeigerCounterController(_mockContext.Object, _mockCounter.Object);
            Assert.AreEqual(samples[0], controller.GetSample().Result.Value);
            Assert.AreEqual(samples[1], controller.GetSample().Result.Value);

            // Validate
            _mockCounter.VerifyAll();
            _mockContext.VerifyAll();
        }

        [Test]
        public void When_GetAllSamples_Expect_AllReturned()
        {
            // Test data
            var dt = new DateTime(2018, 10, 10, 12, 0, 0);
            var samples1 = new List<RadiationSample>
            {
                new RadiationSample { LastCalc = dt, Alpha = 12.0, Beta = 18.0, Gamma = 3.5, Samples = 12, Id = 1},
                new RadiationSample { LastCalc = dt.AddSeconds(30), Alpha = 1.0, Beta = 8.0, Gamma = 5, Samples = 2, Id = 2}
            };
            var samples2 = new List<RadiationSample>(samples1)
            {
                new RadiationSample { LastCalc = dt.AddSeconds(40), Alpha = 6.0, Beta = 9.0, Gamma = 10, Samples = 10, Id = 3 }
            };

            // Expect to calls to calc samples
            _mockContext.SetupSequence(x => x.GetSamplesListAsync()).Returns(Task.FromResult(samples1)).Returns(Task.FromResult(samples2));

            // now create the controller and test
            var controller = new GeigerCounterController(_mockContext.Object, _mockCounter.Object);
            Assert.AreEqual(samples1, controller.GetAllSamples().Result.Value);
            Assert.AreEqual(samples2, controller.GetAllSamples().Result.Value);

            _mockCounter.VerifyAll();
            _mockContext.VerifyAll();
        }


    }
}
