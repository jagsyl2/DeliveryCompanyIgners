using System;
using Topshelf;
using Unity;
using Microsoft.AspNetCore.Hosting;

namespace DeliveryCompany.WebApiTopShelf
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityDiContainerProvider().GetContainer();
            var appHost = container.Resolve<AppHost>();

            var rc = HostFactory.Run(x =>
            {
                x.Service<AppHost>(s =>
                {
                    s.ConstructUsing(sf => appHost);
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("DCIgners.WebApi.TopShelf service");
                x.SetDisplayName("DeliveryCompanyIgners.WebApi.TopShelf");
                x.SetServiceName("DeliveryCompanyIgners.WebApi.TopShelf");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
