using System;

namespace DeliveryCompany.BusinessLayer.SpaceTimeProviders
{
    public interface ITimeProvider
    {
        public DateTime Now { get; }
    }
}
