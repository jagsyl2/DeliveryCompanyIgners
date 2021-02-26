using DeliveryCompany.BusinessLayer.Serializers;
using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using DeliveryCompany.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeliveryCompany.BusinessLayer
{
    public interface IWaybillsDeserialize
    {
        string DeserializeWayBills();
    }

    public class WaybillsDeserialize : IWaybillsDeserialize
    {
        private IJsonSerializer _jsonSerializer;
        private ITimeProvider _fastForwardTimeProvider;

        public WaybillsDeserialize(
            IJsonSerializer jsonSerializer,
            ITimeProvider fastForwardTimeProvider)
        {
            _jsonSerializer = jsonSerializer;
            _fastForwardTimeProvider = fastForwardTimeProvider;
        }

        public string DeserializeWayBills()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}/shipping_lists";

            if (!File.Exists(path))
            {
                return null;
            }
            //var date = _fastForwardTimeProvider.Now.ToString("yyyy-MM-dd");
            //var filePath = Path.Combine($"{path}.{id}_{date}.json");
            var filePath = Path.Combine($"{path}.9_2170-08-05.json");
            var wayBills = _jsonSerializer.Deserialize(filePath);




            //var date = _fastForwardTimeProvider.Now.ToString("yyyy-MM-dd");
            //var filePath2 = Path.Combine(path.FullName, $"{vehicle.DriverId}_{date}.json");
            return wayBills;
        }
    }
}