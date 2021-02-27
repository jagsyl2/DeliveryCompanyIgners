using DeliveryCompany.DataLayer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DeliveryCompany.BusinessLayer.Serializers
{
    public interface IJsonSerializer
    {
        public void Serialize(string filePath, List<Package> dataSet);
        public List<JsonLocationData> DeserializeLocation(string locationData);
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
    }
}
