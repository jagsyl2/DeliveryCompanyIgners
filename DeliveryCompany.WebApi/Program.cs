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
                        Name = "Jakub Bulczak",
                        Email = "kuba@codementors.pl",
                        Url = new Uri("https://www.linkedin.com/in/jakub-bulczak-21873064/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use some license",
                        Url = new Uri("https://webapiexamples.project.com/license")
                    }
                });

            //Descriptions from summaries requires checking up a checkbox: RightClick on project -> Properties -> Build -> XML Documentation file . Leave the default path value.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            genOptions.IncludeXmlComments(xmlPath);

            //If excepton of "file not found" is being thrown, remember to put following nodes into csproj file:
            /*
            <PropertyGroup>
                <GenerateDocumentationFile>true</GenerateDocumentationFile>
            </PropertyGroup>

            <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
                <DocumentationFile>bin\$(Configuration)\$(AssemblyName).xml</DocumentationFile>
            </PropertyGroup>
             */
        }
    }
}
