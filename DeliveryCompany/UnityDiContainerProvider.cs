using DeliveryCompany.BusinessLayer;
using DeliveryCompany.BusinessLayer.Distances;
using DeliveryCompany.BusinessLayer.Notifications;
using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using DeliveryCompany.DataLayer;
using System;
using Unity;
using Unity.Injection;

namespace DeliveryCompany
{
    public class UnityDiContainerProvider
    {
        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IDatabaseManagmentService, DatabaseManagmentService>();
            container.RegisterType<IIoHelperRegisterUser, IoHelperRegisterUser>();
            container.RegisterType<IIoHelperAddVehicle, IoHelperAddVehicle>();
            container.RegisterType<IIoHelperAddPackage, IoHelperAddPackage>();
            container.RegisterType<IPackageService, PackageService>();
            container.RegisterType<IVehicleService, VehicleService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IIoHelper, IoHelper>();
            container.RegisterType<IMenu, Menu>();

            container.RegisterType<ILocationService, LocationService>();
            container.RegisterType<IJsonSerializer, JsonSerializer>();
            container.RegisterType<IPackageStatusOnTheGoService, PackageStatusOnTheGoService>();
            container.RegisterType<ICourierRatingsService, CourierRatingsService>();

            container.RegisterSingleton<ITimeProvider, FastForwardTimeProvider>();
            container.RegisterSingleton<ITimerSheduler, TimerSheduler>();

            container.RegisterSingleton<IWaybillsService, WaybillsService>();
            container.RegisterSingleton<INotificationService, NotificationService>();

            container.RegisterType<Func<IDeliveryCompanyDbContext>>(
                new InjectionFactory(ctx => new Func<IDeliveryCompanyDbContext>(() => new DeliveryCompanyDbContext())));

            return container;
        }
    }
}
