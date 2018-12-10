using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SEO.Core.Model;
using SEO.Core.Process.Processing;

namespace SEO.Core.SampleWebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {           
            services.AddSingleton<IProcessing, Processing>();            
            services.AddSingleton<IRedisCache, RedisCache>();

            services.AddTransient<IAnalysis, Analysis>();

            services.Configure<RedisCacheSettings>(Configuration.GetSection("RedisCacheSettings"));
            services.AddOptions();

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration["RedisCacheSettings:Host"].ToString();
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}