using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace DeliveryCompany.TopShelf
{
    public class AppHost
    {
        private IWebHost _webApiHost;

        private readonly IUnityContainer _container;

        public AppHost(IUnityContainer container)
        {
            _container = container;
        }

        public void Start()
        {
            _webApiHost = WebHost
                .CreateDefaultBuilder()
                .UseUnityServiceProvider(_container)
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                    services.AddSwaggerGen(SwaggerDocsConfig);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    app.UseCors();
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeliveryCompanyIgners V1");
                        c.RoutePrefix = string.Empty;
                    });
                })
                .UseUrls("http://*:10500")
                .Build();

                _webApiHost.RunAsync();
        }

        public void Stop()
        {
            _webApiHost.StopAsync().Wait();
        }

        private static void SwaggerDocsConfig(SwaggerGenOptions genOptions)
        {
            genOptions.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DeliveryCompanyIgners",
                    Description = "Aplication of delivery company - ASP.NET Core Web API",
                    TermsOfService = new Uri("https://deliverycompanyigners.project.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Sylwia Ignerowicz",
                        Email = "jagsyl@poczta.onet.pl",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use some license",
                        Url = new Uri("https://deliverycompanyigners.project.com/license")
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            genOptions.IncludeXmlComments(xmlPath);
        }

    }
}
