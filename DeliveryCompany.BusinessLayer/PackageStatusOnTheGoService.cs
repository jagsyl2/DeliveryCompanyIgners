using DeliveryCompany.BusinessLayer.Notifications;
using DeliveryCompany.DataLayer.Models;

namespace DeliveryCompany.BusinessLayer
{
    public interface IPackageStatusOnTheGoService
    {
        public void ChangingPackageStatusAtTheBeginningOfJourney();
        public void ChangingPackageStatusAtTheEndOfJourney();
    }

    public class PackageStatusOnTheGoService : IPackageStatusOnTheGoService
    {
        private IPackageService _packageService;
        private INotificationService _notificationService;

        public PackageStatusOnTheGoService(
            IPackageService packageService,
            INotificationService notificationService)
        {
            _packageService = packageService;
            _notificationService = notificationService;
        }

        public void ChangingPackageStatusAtTheBeginningOfJourney()
        {
            var todaysPackages = _packageService.GetPackagesWithStatus(StateOfPackage.Given);
            _packageService.UpdatePackages(todaysPackages, StateOfPackage.OnTheWay);
        }

        public void ChangingPackageStatusAtTheEndOfJourney()
        {
            var todaysPackages = _packageService.GetPackagesWithStatus(StateOfPackage.OnTheWay);
            _packageService.UpdatePackages(todaysPackages, StateOfPackage.Received);

            foreach (var package in todaysPackages)
            {
                _notificationService.NotifyOfPackageDelivery(package);
            }
        }
    }
}
