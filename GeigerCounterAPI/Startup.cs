using GeigerCounterAPI.Implementation;
using GeigerCounterAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeigerCounterAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        // ReSharper disable once UnusedParameter.Local
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();             
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
//            services.AddDbContext<GeigerCounterContext>(opt => opt.UseInMemoryDatabase("RadiationCounter"));
            services.AddDbContext<GeigerCounterContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RadiationCounterDatabase")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IRadiationCounter, RadiationCounter>((ctx) => new RadiationCounter(new TimeProvider()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
