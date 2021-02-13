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
        private readonly Func<IDeliveryCompanyDbContext> _dbContextFactoryMethod;

        public DatabaseManagmentService(Func<IDeliveryCompanyDbContext> deliveryCompanyDbContextFactoryMethod)
        {
            _dbContextFactoryMethod = deliveryCompanyDbContextFactoryMethod;
        }

        public void EnsureDatabaseCreation()
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
