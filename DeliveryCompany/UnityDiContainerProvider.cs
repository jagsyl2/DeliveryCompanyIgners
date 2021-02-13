using DeliveryCompany.BusinessLayer;
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
            container.RegisterType<IIoHelperRegisterUser, IIoHelperRegisterUser>();
            container.RegisterType<IIoHelperAddVehicle, IoHelperAddVehicle>();
            container.RegisterType<IIoHelperAddPackage, IoHelperAddPackage>();
            container.RegisterType<IPackageService, PackageService>();
            container.RegisterType<IVehicleService, VehicleService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IIoHelper, IoHelper>();
            container.RegisterType<IMenu, Menu>();

            container.RegisterType<Func<IDeliveryCompanyDbContext>>(
                new InjectionFactory(ctx => new Func<IDeliveryCompanyDbContext>(() => new DeliveryCompanyDbContext())));

            //container.RegisterFactory<Func<IDeliveryCompanyDbContext>>(ctx => new Func<IDeliveryCompanyDbContext>(() => new DeliveryCompanyDbContext()));
            return container;
        }
    }
}
