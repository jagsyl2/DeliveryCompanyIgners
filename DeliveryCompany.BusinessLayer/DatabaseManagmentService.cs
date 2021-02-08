using DeliveryCompany.DataLayer;

namespace DeliveryCompany.BusinessLayer
{
    public class DatabaseManagmentService
    {
        public void EnsureDatabaseCreation()
        {
            using (var context = new DeliveryCompanyDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
