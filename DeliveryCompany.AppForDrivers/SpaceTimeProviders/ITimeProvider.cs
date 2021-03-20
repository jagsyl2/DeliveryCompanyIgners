using System;

namespace DeliveryCompany.AppForDrivers.SpaceTimeProviders
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
