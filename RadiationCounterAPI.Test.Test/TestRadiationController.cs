using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Moq;
using NUnit.Framework;
using RadiationCounterAPI.Models;
using RadiationCounterAPI.Implementation;;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RadiationCounterAPI.Test.Test
{
    [TestFixture]
    class TestRadiationController
    {
        [Test]
        void When_DoStuff_Expect_StuffDone()
        {
            var mockCtxt = new Mock<RadiationCounterContext>(MockBehavior.Strict);

            // Create initial data
            DateTime dt = new DateTime(2018, 10, 10, 12,, 0, 0);
            var data = new List<RadiationSample>
            {
                new RadiationSample { LastCalc = dt, Alpha = 12.0, Beta = 18.0, Gamma = 3.5, Samples = 12, Id = 1},
                new RadiationSample { LastCalc = dt.AddSeconds(30), Alpha = 1.0, Beta = 8.0, Gamma = 5, Samples = 2, Id = 2},
                new RadiationSample { LastCalc = dt.AddSeconds(90), Alpha = 3.0, Beta = 3.0, Gamma = 0, Samples = 150, Id = 3},
            };

            DbSet<RadiationSample> set = new DbSet<RadiationSample>();

            // Create a mock context and its containing DBSet
            var set2 = new Mock<DbSet<RadiationSample>>();
//            var set2 = new Mock<DbSet<RadiationSample>>().SetupData(data);

            var context = new Mock<RadiationCounterContext>();
            context.Setup(c => c.Samples).Returns(set.Object);

            // Create a BlogsController and invoke the Index action
            var controller = new BlogsController(context.Object);
            var result = await controller.Index();

            // Check the results
            var blogs = (List<Blog>)result.Model;
            Assert.AreEqual(3, blogs.Count());
            Assert.AreEqual("AAA", blogs[0].Name);
            Assert.AreEqual("BBB", blogs[1].Name);
            Assert.AreEqual("CCC", blogs[2].Name);

        }
    }
}
