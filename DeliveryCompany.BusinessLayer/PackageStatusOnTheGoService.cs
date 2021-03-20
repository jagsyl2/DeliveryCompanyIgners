using DeliveryCompany.BusinessLayer.Notifications;
using DeliveryCompany.DataLayer.Models;

namespace DeliveryCompany.BusinessLayer
{
    public interface IPackageStatusOnTheGoService
    {
        void ChangingPackageStatusAtTheBeginningOfJourney();
        void ChangingPackageStatusAtTheEndOfJourney();
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
            _packageService.UpdatePackagesOnAutomaticWaybill(todaysPackages, StateOfPackage.OnTheWay);
        }

        public void ChangingPackageStatusAtTheEndOfJourney()
        {
            var todaysPackages = _packageService.GetPackagesWithStatusOnAutomaticWaybill(StateOfPackage.OnTheWay);
            _packageService.UpdatePackages(todaysPackages, StateOfPackage.Received, 5);

            foreach (var package in todaysPackages)
            {
                _notificationService.NotifyOfPackageDelivery(package);
            }
        }
    }
}
