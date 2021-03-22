using DeliveryCompany.AppForDrivers.Distances;
using DeliveryCompany.AppForDrivers.SpaceTimeProviders;
using Unity;

namespace DeliveryCompany.AppForDrivers
{
    public class UnityDiContainerProvider
    {
        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IIoHelper, IoHelper>();
            container.RegisterType<ILocationService, LocationService>();
            container.RegisterType<IPackageServices, PackageServices>();

            container.RegisterSingleton<ITimeProvider, FastForwardTimeProvider>();

            return container;
        }
    }
}
