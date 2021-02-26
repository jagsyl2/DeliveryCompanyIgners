using DeliveryCompany.DataLayer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DeliveryCompany.BusinessLayer.Serializers
{
    public interface IJsonSerializer
    {
        void Serialize(string filePath, List<Package> dataSet);
        List<JsonLocationData> DeserializeLocation(string locationData);
        string Deserialize(string filePath);
    }

    public class JsonSerializer : IJsonSerializer
    {
        public void Serialize(string filePath, List<Package> dataSet)
        {
            var jsonData = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
        
        public List<JsonLocationData> DeserializeLocation(string locationData)
        {
            var data = JsonConvert.DeserializeObject<List<JsonLocationData>>(locationData);

            return data;
        }
        public string Deserialize(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            //var wayBill = JsonConvert.DeserializeObject<List<Package>>(jsonData);

            return jsonData;
        }
    }
}
