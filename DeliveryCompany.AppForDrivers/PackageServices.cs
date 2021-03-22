using DeliveryCompany.AppForDrivers.Models;
using DeliveryCompany.AppForDrivers.SpaceTimeProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DeliveryCompany.AppForDrivers
{
    public interface IPackageServices
    {
        DateTime GetStartTime();
        void MarkDeliveredPackage(List<Package> waybill, List<WaybillItem> waybillItems);
    }

    public class PackageServices : IPackageServices
    {
        private readonly ITimeProvider _fastForwardTimeProvider;
        private readonly IIoHelper _ioHelper;

        public PackageServices(
            ITimeProvider fastForwardTimeProvider,
            IIoHelper ioHelper)
        {
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _ioHelper = ioHelper;
        }

        public DateTime GetStartTime()
        {
            if (0 < _fastForwardTimeProvider.Now.Hour && _fastForwardTimeProvider.Now.Hour < 8)
            {
                return _fastForwardTimeProvider.Now;
            }
            else
            {
                return _fastForwardTimeProvider.Now.Date.AddHours(8);
            }
        }

        public void MarkDeliveredPackage(List<Package> waybill, List<WaybillItem> waybillItems)
        {
            var deliveredPackageId = _ioHelper.GetIntFromUser("Provide the number of the delivered package:");
            if (!waybill.Any(x => x.Id == deliveredPackageId))
            {
                Console.WriteLine("There is no such package.");
                return;
            }
            if (waybill.Any(x => x.Id == deliveredPackageId && x.State == StateOfPackage.DeliveredManually))
            {
                Console.WriteLine("Package has already been delivered.");
                return;
            }
            if (waybill.Any(x => x.Id == deliveredPackageId && x.ModeWaybill == ModeOfWaybill.Automatic))
            {
                CheckPackage(deliveredPackageId, waybill, waybillItems);
                return;
            }
            else
            {
                UpdatePackageStatus(deliveredPackageId, waybill, waybillItems);
                return;
            }
        }

        private void CheckPackage(int deliveredPackageId, List<Package> waybill, List<WaybillItem> waybillItems)
        {
            var package = GetPackage(deliveredPackageId);
            if (package == null)
            {
                Console.WriteLine("There is no such package.");
                return;
            }
            if (package.State == StateOfPackage.Received)
            {
                Console.WriteLine("Package has already been delivered.");
                return;
            }
            else
            {
                UpdatePackageStatus(deliveredPackageId, waybill, waybillItems);
                return;
            }
        }

        private Package GetPackage(int deliveredPackageId)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($@"http://localhost:10500/api/packages/{deliveredPackageId}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseObject = JsonConvert.DeserializeObject<Package>(responseText);
                    return responseObject;
                }
                else
                {
                    Console.WriteLine("Something went wrong");
                    return null;
                }
            }
        }

        private void UpdatePackageStatus(int deliveredPackageId, List<Package> _waybill, List<WaybillItem> waybillItems)
        {
            var package = _waybill.FirstOrDefault(x => x.Id == deliveredPackageId);

            package.State = StateOfPackage.DeliveredManually;
            package.DeliveryDate = _fastForwardTimeProvider.Now;
            var estimatedDeliveryTime = waybillItems
                .Where(x => x.Package.Id == deliveredPackageId && x.TypeOfPackage == TypeOfPackages.ToBeDelivered)
                .Select(x => x.EstimatedDeliveryTime)
                .FirstOrDefault();

            package.CourierRating = CountCouriersRating(package, estimatedDeliveryTime);

            var content = new StringContent(JsonConvert.SerializeObject(package), Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PutAsync($@"http://localhost:10500/api/packages/status/{deliveredPackageId}", content).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine($"Success.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Failed again. Status code: {response.StatusCode}");
                    return;
                }
            }
        }

        private int CountCouriersRating(Package package, DateTime estimatedDeliveryTime)
        {
            var deviation = (package.DeliveryDate - estimatedDeliveryTime).TotalMinutes;

            if (-10 < deviation && deviation < 10)
            {
                return 5;
            }
            if (-20 < deviation && deviation < 20)
            {
                return 4;
            }
            if (-30 < deviation && deviation < 30)
            {
                return 3;
            }
            if (-40 < deviation && deviation < 40)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
