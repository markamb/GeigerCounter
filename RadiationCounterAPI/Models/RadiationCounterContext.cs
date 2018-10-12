using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RadiationCounterAPI.Models
{
    public class RadiationCounterContext : DbContext
    {
        public RadiationCounterContext(DbContextOptions<RadiationCounterContext> options) : base(options)
        {
        }

        /// <summary>
        /// Stores the set of all samples made up to this point
        /// </summary>
        public DbSet<RadiationSample> Samples { get; set; }

        public virtual List<RadiationSample> GetSamplesList()
        {
            return Samples.ToList();
        }

        public virtual void AddSample(RadiationSample sample)
        {
            Samples.Add(sample);
        }
        
    }
}
