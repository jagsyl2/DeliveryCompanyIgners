using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.BusinessLayer.Serializers;
using Quartz;
using System;
using System.Threading.Tasks;

namespace DeliveryCompany.BusinessLayer.ScheduledTask
{
    public class ScheduledTaskJob : IJob
    {
        private WaybillsService _waybillsService = new WaybillsService(new LocationService(), new VehicleService(), new PackageService(), new JsonSerializer(), new UserService());

        Task IJob.Execute(IJobExecutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Task taskA = new Task(() => _waybillsService.CreateWaybills());
            taskA.Start();
            return taskA;
        }
    }
}
