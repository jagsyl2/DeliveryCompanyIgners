using Microsoft.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Unity.Microsoft.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;

namespace DeliveryCompany.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityDiContainerProvider().GetContainer();

            WebHost
                .CreateDefaultBuilder()
                .UseUnityServiceProvider(container)
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                    services.AddSwaggerGen(SwaggerDocsConfig);
                })
                .Configure(app => {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    app.UseCors();
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiExample V1");
                        c.RoutePrefix = string.Empty;
                    });
                })
                .UseUrls("http://*:10500")
                .Build()
                .Run();
        }

        private static void SwaggerDocsConfig(SwaggerGenOptions genOptions)
        {
            genOptions.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WebApiExample",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://webapiexamples.project.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Sylwia Ignerowicz",
                        Email = "jagsyl@poczta.onet.pl",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use some license",
                        Url = new Uri("https://webapiexamples.project.com/license")
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            genOptions.IncludeXmlComments(xmlPath);
        }
    }
}
