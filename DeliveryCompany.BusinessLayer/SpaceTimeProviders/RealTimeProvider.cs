using System;

namespace DeliveryCompany.BusinessLayer.SpaceTimeProviders
{
    public class RealTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
