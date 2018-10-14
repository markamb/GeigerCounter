using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GeigerCounterAPI.Models
{
    public class GeigerCounterContext : DbContext
    {
        public GeigerCounterContext(DbContextOptions<GeigerCounterContext> options) : base(options)
        {
        }

        /// <summary>
        /// Stores the set of all samples made up to this point
        /// </summary>
        public DbSet<RadiationSample> Samples { get; set; }

        /// <summary>
        /// Ensure that the database is created and initialised with any seed data (none at present)
        /// Not that this is for development use only
        /// </summary>
        public virtual void EnsureDatabaseCreated()
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Return a list of all samples stored to date
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<RadiationSample>> GetSamplesListAsync()
        {
            return await Samples.ToListAsync();
        }

        /// <summary>
        /// Add a new sample to the database
        /// </summary>
        /// <param name="sample"></param>
        public virtual async Task AddSampleAsync(RadiationSample sample)
        {
            await Samples.AddAsync(sample);
        }
        
    }
}
