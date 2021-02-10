using DeliveryCompany.BusinessLayer.Distances;

namespace DeliveryCompany.BusinessLayer.Models
{
    public class CourierLocationsAlongTheWay
    {
        public LocationCoordinates StartingPlace;
        //internal LocationCoordinates StartingPlaceLon;
        internal LocationCoordinates CourierCurrentLocation;
        //internal LocationCoordinates CourierCurrentLocationLon;
        internal bool FirstPackageForCourier = true;
        internal LocationCoordinates RecipientFirstPackage;
        //internal LocationCoordinates RecipientFirstPackageLon;
        internal LocationCoordinates RecipientCurrentPackage;
        //internal LocationCoordinates RecipientCurrentPackageLon;
    }
}
