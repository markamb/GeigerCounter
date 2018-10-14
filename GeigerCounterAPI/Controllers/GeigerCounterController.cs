using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GeigerCounterAPI.Models;
using GeigerCounterAPI.Implementation;
using System.Threading.Tasks;

namespace GeigerCounterAPI.Controllers
{
    /// <summary>
    /// The main controller for our application. Handles all requests.
    /// </summary>
    [Route("")]
    [ApiController]
    public class GeigerCounterController : ControllerBase
    {
        private readonly GeigerCounterContext _context;
        private readonly IRadiationCounter _counter;             // our singleton object for accumulating readings across sessions

        public GeigerCounterController(GeigerCounterContext context, IRadiationCounter counter)
        {
            _context = context;
            _counter = counter;
        }


        /// <summary>
        /// Supply an external radiation reading
        /// 
        /// </summary>
        /// <remarks>
        /// Supply an external radiation reading as a count of the number of alpha, beta and gamma particles detected.
        ///
        /// Sample Request:
        ///
        /// POST particles
        /// {
        ///     "alpha": 12,
        ///     "beta": 1,
        ///     "gamma": 0 
        /// }
        /// </remarks>
        ///
        /// <param name="reading"></param>
        /// 
        /// <returns>None</returns>
        /// <response code="200">Reading recorded</response>
        /// <response code="400">If not all counts are provided</response>    
        [HttpPost("/particles")]
        public ActionResult TakeReading([FromBody]ParticleReading reading)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (reading == null)
                return BadRequest();
            _counter.TakeReading(reading);
            return Ok();
        }

        /// <summary>
        /// Returns the average rate of particle detection since the last sample.
        /// </summary>
        /// <remarks>
        /// Returns the average rate of particle detection since the last average was requested.
        /// The rates are provided as the average number of alpha, beta and gamma particles per second over the recording time.
        /// LastCalc shows the start time of each calculation.
        /// Samples shows the number of samples recorded during the period.
        /// 
        /// Sample Request:
        /// GET /radiation/average
        /// 
        /// </remarks>
        /// 
        /// <returns>A radiation sample</returns>
        /// <response code="200">A radiation sample has been returned</response> 
        [HttpGet("/radiation/average", Name = "GetSample")]
        public async Task<ActionResult<RadiationSample>> GetSample()
        {
            // Calculate the particle counts per second since the last sample was taken,
            // store it then return it.
            var sample = _counter.CalcSample();
            await _context.AddSampleAsync(sample);
            await _context.SaveChangesAsync();
            return sample;
        }

        /// <summary>
        /// Returns all historical samples previously returned
        /// </summary>
        /// <remarks>
        /// Returns a list of all historical average radiation calculations.
        /// The rates are provided as the average number of alpha, beta and gamma particles per second over the recording time.
        /// LastCalc shows the start time of each calculation.
        /// Samples shows the number of samples recorded during the period.
        ///
        /// Sample Request:
        /// GET radiation/results
        /// </remarks>
        /// 
        /// <returns>A list of all historical radiation samples</returns>
        /// <response code="200">Radiation samples have been returned</response> 
        [HttpGet("/radiation/results", Name = "GetResults")]
        public async Task<ActionResult<List<RadiationSample>>> GetAllSamples()
        {
            // Note: If no samples have been read yet we return back a valid but empty list
            return await _context.GetSamplesListAsync();
        }
    }
}
