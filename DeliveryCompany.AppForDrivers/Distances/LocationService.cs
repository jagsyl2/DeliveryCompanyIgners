using Geolocation;

namespace DeliveryCompany.AppForDrivers.Distances
{
    public interface ILocationService
    {
        double GetDistanceBetweenTwoPlaces(LocationCoordinates originLocation, double destinationLat, double destinationLon);
    }

    public class LocationService : ILocationService
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
