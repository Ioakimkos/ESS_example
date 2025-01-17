using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ESS_example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();

                    var responseText = "Hello, World!";

                    logger.LogInformation("Received GET request for /");
                    logger.LogInformation("Response: {ResponseText}", responseText);

                    await context.Response.WriteAsync(responseText);
                });
            });
        }
    }
}