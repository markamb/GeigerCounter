using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RadiationCounterAPI.Models;
using RadiationCounterAPI.Implementation;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RadiationCounterAPI.Test")]

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadiationCounterAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class RadiationController : ControllerBase
    {
        private readonly RadiationCounterContext _context;
        private readonly IRadiationCounter _counter;             // our singleton object for accumulating readings across sessions

        public RadiationController(RadiationCounterContext context, IRadiationCounter counter)
        {
            _context = context;
            _counter = counter;
        }

        // POST:  /particles
        [HttpPost("/particles")]
        public void TakeReading([FromBody]ParticleReading reading)
        {
              _counter.TakeReading(reading);
        }

        // GET radiation/average
        [HttpGet("/radiation/average", Name = "GetSample")]
        public ActionResult<RadiationSample> GetSample()
        {
            // Calculate the particle counts per second since the last sample was taken,
            // store then return it.
            var sample = _counter.CalcSample();
            _context.AddSample(sample);
            _context.SaveChanges();
            return sample;
        }

        // GET radiation/results
        [HttpGet("/radiation/results", Name = "GetResults")]
        public ActionResult<List<RadiationSample>> GetAllSamples()
        {
            // Note: If no samples have been read yet we return back a valid but empty list
            return _context.GetSamplesList();
        }
    }
}
