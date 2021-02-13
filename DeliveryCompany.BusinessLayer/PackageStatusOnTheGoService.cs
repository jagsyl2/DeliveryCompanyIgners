using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.DataLayer.Models;
using System;

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

        public PackageStatusOnTheGoService(IPackageService packageService)
        {
            _packageService = packageService;
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
        }
    }
}
