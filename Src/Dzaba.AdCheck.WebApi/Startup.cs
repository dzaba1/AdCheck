using System.Text.Json.Serialization;
using Dzaba.AdCheck.DataAccess;
using Dzaba.AdCheck.DataAccess.SqlServer;
using Dzaba.AdCheck.Diff;
using Dzaba.AdCheck.WebApi.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.OpenApi.Models;

namespace Dzaba.AdCheck.WebApi
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
            services.RegisterAdDiff();
            services.RegisterAdDatabase(c => Configuration.GetConnectionString("Default"));
            services.RegisterAdSqlServer();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IClaimsTransformation, RolesTransformer>();

            // Windows auth
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            
            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdCheck", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", "AdCheck v1");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
