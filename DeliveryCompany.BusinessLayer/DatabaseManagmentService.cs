using DeliveryCompany.DataLayer;
using System;

namespace DeliveryCompany.BusinessLayer
{
    public interface IDatabaseManagmentService
    {
        void EnsureDatabaseCreation();
    }

    public class DatabaseManagmentService : IDatabaseManagmentService
    {
        private readonly Func<IDeliveryCompanyDbContext> _deliveryCompanyDbContextFactoryMethod;

        public DatabaseManagmentService(Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _deliveryCompanyDbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public void EnsureDatabaseCreation()
        {
            using (var context = _deliveryCompanyDbContextFactoryMethod())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
