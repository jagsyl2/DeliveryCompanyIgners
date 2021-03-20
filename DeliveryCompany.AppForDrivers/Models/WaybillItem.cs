using System;

namespace DeliveryCompany.AppForDrivers.Models
{
    public enum TypeOfPackages
    {
        ForPickup = 1,
        ToBeDelivered = 2,

    }
    public class WaybillItem
    {
        public Package Package;
        public double Distance;
        public double Time;
        public DateTime EstimatedDeliveryTime;
        public TypeOfPackages TypeOfPackage;
    }
}
