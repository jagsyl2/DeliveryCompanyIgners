using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.DataLayer.Models;
using Geolocation;
using RestSharp;
using System.Collections.Generic;

namespace DeliveryCompany.BusinessLayer
{
    public interface ILocationService
    {
        public List<JsonLocationData> ChangeLocationToCoordinates(User user);
        public Dictionary<int, double> CountDistancesFromPackageToCourier(List<User> couriers, Package package);
        public string GetCoordinatesForAddress(string country, string city, string street, string building);
        public double Location(JsonLocationData originLocation, JsonLocationData destinationLocation);
    }

    public class LocationService : ILocationService
    {
        private JsonSerializer _jsonSerializer = new JsonSerializer();

        public List<JsonLocationData> ChangeLocationToCoordinates(User user)
        {
            var location = GetCoordinatesForAddress("Polska", user.City, user.Street, user.StreetNumber);
            var locationCoordinates = _jsonSerializer.Deserialize(location);
            return locationCoordinates;
        }

        public Dictionary<int, double> CountDistancesFromPackageToCourier(List<User> couriers, Package package)
        {
            var distances = new Dictionary<int, double>();
            var packageLocationCoordinates = ChangeLocationToCoordinates(package.Sender);

            foreach (var courier in couriers)
            {
                var courierLocationCoordinates = ChangeLocationToCoordinates(courier);
                var distance = Location(packageLocationCoordinates[0], courierLocationCoordinates[0]);

                distances[courier.Id] = distance;
            }

            return distances;
        }

        public string GetCoordinatesForAddress(string country, string city, string street, string building)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/?q={street}+{building}+{city}+{country}&format=json&limit=1");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var data = response.Content;

            return data;
        }

        public double Location(JsonLocationData originLocation, JsonLocationData destinationLocation)
        {
            Coordinate origin = new Coordinate() { Latitude = originLocation.lat, Longitude = originLocation.lon };
            Coordinate destination = new Coordinate() { Latitude = destinationLocation.lat, Longitude = destinationLocation.lon };

            double distance = GeoCalculator.GetDistance(origin, destination, 1, DistanceUnit.Kilometers);
            
            return distance;
        }
    }
}
