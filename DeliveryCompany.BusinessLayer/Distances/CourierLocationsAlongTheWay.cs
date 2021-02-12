using DeliveryCompany.BusinessLayer.Distances;

namespace DeliveryCompany.BusinessLayer.Models
{
    public class CourierLocationsAlongTheWay
    {
        public LocationCoordinates StartingPlace;
        internal LocationCoordinates CourierCurrentLocation;
        internal bool FirstPackageForCourier = true;
        internal LocationCoordinates RecipientFirstPackage;
        internal LocationCoordinates RecipientCurrentPackage;

        //internal LocationCoordinates StartingPlaceLon;
        //internal LocationCoordinates CourierCurrentLocationLon;
        //internal LocationCoordinates RecipientFirstPackageLon;
        //internal LocationCoordinates RecipientCurrentPackageLon;
    }
}
