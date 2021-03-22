using System;

namespace DeliveryCompany.AppForDrivers.SpaceTimeProviders
{
    public class RealTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
