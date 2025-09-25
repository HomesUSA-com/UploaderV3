namespace Husa.Uploader.Api
{
    using System;
    using Husa.Extensions.Api;
    using Husa.Extensions.Api.Client;
    using Husa.Extensions.Api.Cors;
    using Husa.Extensions.Api.Middleware;
    using Husa.Extensions.Authorization.Extensions;
    using Husa.Uploader.Api.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Startup
    {
        private readonly IWebHostEnvironment environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .BindOptions()
                .AddApplicationServices()
                .AddDataRepositories()
                .AddMvcOptions()
                .UseAuthorizationContext()
                .BindIdentitySettings()
                .ControllerConfiguration()
                .RegisterAutoMapper()
                .ConfigureHttpClients()
                .ConfigureWebDriver()
                .SwaggerConfiguration()
                .ConfigureWorker();
            services.RegisterDelegatedServices();
            services.AddHeaderPropagation();

            services
                .AddHttpContextAccessor()
                .AddControllers();

            if (!this.environment.IsDevelopment())
            {
                services.AddApplicationInsightsTelemetry();
            }

            services.ConfigureApiClients(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Husa.Uploader.Api v1"));
            }

            app.UseMiddleware<ProductVersionHeaderMiddleware>();
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.ConfigureCors();
            app.ConfigureCultureAndLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
