using Geolocation;

namespace DeliveryCompany.AppForDrivers.Distances
{

    public class LocationService
    {
        public double GetDistanceBetweenTwoPlaces(LocationCoordinates originLocation, double destinationLat, double destinationLon)
        {
            Coordinate origin = new Coordinate() { Latitude = originLocation.Lat, Longitude = originLocation.Lon };
            Coordinate destination = new Coordinate() { Latitude = destinationLat, Longitude = destinationLon };

            double distance = GeoCalculator.GetDistance(origin, destination, 1, DistanceUnit.Kilometers);

            return distance;
        }
    }
}
