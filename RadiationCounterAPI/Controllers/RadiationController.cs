using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RadiationCounterAPI.Models;
using RadiationCounterAPI.Implementation;
using System.Threading.Tasks;

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

        // GET /status
        [HttpGet("/status", Name = "Status")]
        public ActionResult<string> GetStatus()
        {
            return "OK";
        }

        // POST:  /particles
        [HttpPost("/particles")]
        public void TakeReading([FromBody]ParticleReading reading)
        {
              _counter.TakeReading(reading);
        }

        // GET radiation/average
        [HttpGet("/radiation/average", Name = "GetSample")]
        public async Task<ActionResult<RadiationSample>> GetSample()
        {
            // Calculate the particle counts per second since the last sample was taken,
            // store then return it.
            var sample = _counter.CalcSample();
            _context.AddSampleAsync(sample);
            await _context.SaveChangesAsync();
            return sample;
        }

        // GET radiation/results
        [HttpGet("/radiation/results", Name = "GetResults")]
        public async Task<ActionResult<List<RadiationSample>>> GetAllSamples()
        {
            // Note: If no samples have been read yet we return back a valid but empty list
            return await _context.GetSamplesListAsync();
        }
    }
}
