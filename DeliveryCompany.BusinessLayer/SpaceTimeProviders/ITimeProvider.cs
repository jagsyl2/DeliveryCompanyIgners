using System;

namespace DeliveryCompany.BusinessLayer.SpaceTimeProviders
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
