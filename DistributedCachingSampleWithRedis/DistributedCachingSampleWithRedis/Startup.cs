using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DistributedCachingSampleWithRedis.Core.Caching;
using DistributedCachingSampleWithRedis.Repositories;

namespace DistributedCachingSampleWithRedis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureCachers(services);
            services.AddScoped<IDummyDataRepository, DummyDataRepository>();
        }

        private void ConfigureCachers(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<IMemoryCacher, MemoryCacher>();

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetValue("DistributedCache:Configuration", "localhost");
                options.InstanceName = Configuration.GetValue("DistributedCache:InstanceName", "master");
            });
            services.AddSingleton<IDistributedCacher, DistributedCacher>();
            services.AddTransient<ICacheManager, CacheManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
