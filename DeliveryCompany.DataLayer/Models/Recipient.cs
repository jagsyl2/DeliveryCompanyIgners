using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryCompany.DataLayer.Models
{
    public class Recipient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
