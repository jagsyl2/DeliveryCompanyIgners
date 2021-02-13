using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.DataLayer.Models;
using Geolocation;
using RestSharp;
using System.Collections.Generic;

namespace DeliveryCompany.BusinessLayer.Distances
{
    public interface ILocationService
    {
        public LocationCoordinates ChangeLocationToCoordinates(User user);
        public LocationCoordinates ChangeLocationToCoordinates(string city, string postCode, string street, string streetNumber);
        public Dictionary<int, double> CountDistancesFromPackageToCouriers(List<User> couriers, Package package);
        public string GetCoordinatesForAddress(string country, string city, string postcode, string street, string building);
        public double GetDistanceBetweenTwoPlaces(LocationCoordinates originLocation, LocationCoordinates destinationLocation);
    }

    public class LocationService : ILocationService
    {
        private readonly IJsonSerializer _jsonSerializer;

        public LocationService( IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public LocationCoordinates ChangeLocationToCoordinates(User user)
        {
            var location = GetCoordinatesForAddress("Polska", user.City, user.PostCode, user.Street, user.StreetNumber);
            var locationData = _jsonSerializer.DeserializeLocation(location);

            var locationCoordinates = new LocationCoordinates()
            {
                Lat = locationData[0].lat,
                Lon = locationData[0].lon,
            };
           
            return locationCoordinates;
        }

        public LocationCoordinates ChangeLocationToCoordinates(string city, string postCode, string street, string streetNumber)
        {
            var location = GetCoordinatesForAddress("Polska", city, postCode, street, streetNumber);
            var locationData = _jsonSerializer.DeserializeLocation(location);

            var locationCoordinates = new LocationCoordinates()
            {
                Lat = locationData[0].lat,
                Lon = locationData[0].lon,
            };

            return locationCoordinates;
        }

        public Dictionary<int, double> CountDistancesFromPackageToCouriers(List<User> couriers, Package package)
        {
            var distances = new Dictionary<int, double>();
            //var packageLocationCoordinates = ChangeLocationToCoordinates(package.Sender);
            var packageLocationCoordinates = new LocationCoordinates()
            {
                Lat = package.Sender.lat,
                Lon = package.Sender.lon
            };

            foreach (var courier in couriers)
            {
                var courierLocationCoordinates = new LocationCoordinates()
                {
                    Lat = courier.lat,
                    Lon = courier.lon
                };
                //var courierLocationCoordinates = ChangeLocationToCoordinates(courier);
                var distance = GetDistanceBetweenTwoPlaces(packageLocationCoordinates, courierLocationCoordinates);

                distances[courier.Id] = distance;
            }

            return distances;
        }

        public string GetCoordinatesForAddress(string country, string city, string postalcode, string street, string building)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/?q={street}+{building}+{city}+{country}&{postalcode}&polygon_geojson=1&format=json&limit=1");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var data = response.Content;

            return data;
        }

        public double GetDistanceBetweenTwoPlaces(LocationCoordinates originLocation, LocationCoordinates destinationLocation)
        {
            Coordinate origin = new Coordinate() { Latitude = originLocation.Lat, Longitude = originLocation.Lon };
            Coordinate destination = new Coordinate() { Latitude = destinationLocation.Lat, Longitude = destinationLocation.Lon };

            double distance = GeoCalculator.GetDistance(origin, destination, 1, DistanceUnit.Kilometers);

            return distance;
        }
    }

    //public class LocationService : ILocationService
    //{
    //    private JsonSerializer _jsonSerializer = new JsonSerializer();

    //    public List<JsonLocationData> ChangeLocationToCoordinates(User user)
    //    {
    //        var location = GetCoordinatesForAddress("Polska", user.City, user.PostCode, user.Street, user.StreetNumber);
    //        var locationCoordinates = _jsonSerializer.Deserialize(location);
    //        return locationCoordinates;
    //    }

    //    public List<JsonLocationData> ChangeLocationToCoordinates(Recipient recipient)
    //    {
    //        var location = GetCoordinatesForAddress("Polska", recipient.City, recipient.PostCode, recipient.Street, recipient.StreetNumber);
    //        var locationCoordinates = _jsonSerializer.Deserialize(location);
    //        return locationCoordinates;
    //    }

    //    public Dictionary<int, double> CountDistancesFromPackageToCourier(List<User> couriers, Package package)
    //    {
    //        var distances = new Dictionary<int, double>();
    //        var packageLocationCoordinates = ChangeLocationToCoordinates(package.Sender);

    //        foreach (var courier in couriers)
    //        {
    //            var courierLocationCoordinates = ChangeLocationToCoordinates(courier);
    //            var distance = GetDistanceBetweenTwoPlaces(packageLocationCoordinates[0], courierLocationCoordinates[0]);

    //            distances[courier.Id] = distance;
    //        }

    //        return distances;
    //    }

    //    public string GetCoordinatesForAddress(string country, string city, string postalcode, string street, string building)
    //    {
    //        var client = new RestClient($"https://nominatim.openstreetmap.org/?q={street}+{building}+{city}+{country}&{postalcode}&polygon_geojson=1&format=json&limit=1");
    //        var request = new RestRequest(Method.GET);
    //        IRestResponse response = client.Execute(request);

    //        var data = response.Content;

    //        return data;
    //    }

    //    public double GetDistanceBetweenTwoPlaces(JsonLocationData originLocation, JsonLocationData destinationLocation)
    //    {
    //        Coordinate origin = new Coordinate() { Latitude = originLocation.lat, Longitude = originLocation.lon };
    //        Coordinate destination = new Coordinate() { Latitude = destinationLocation.lat, Longitude = destinationLocation.lon };

    //        double distance = GeoCalculator.GetDistance(origin, destination, 1, DistanceUnit.Kilometers);

    //        return distance;
    //    }
    //}
}
