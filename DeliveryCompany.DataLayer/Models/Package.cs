using System;

namespace DeliveryCompany.DataLayer.Models
{
    public enum PackageSize
    {
        Small = 15,
        Average = 50,
        Large = 150,
    }

    public enum StateOfPackage
    {
        AwaitingPosting,
        Given,
        OnTheWay,
        AwaitingPickup,
        Received
    }

    public class Package
    {
        public int Id { get; set; }
        public Guid Number { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public PackageSize Size { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public StateOfPackage State { get; set; }
        public int VehicleNumber { get; set; }
        //public Recipient Recipient { get; set; }
        public string RecipientName { get; set; }
        public string RecipientSurname { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientStreet { get; set; }
        public string RecipientStreetNumber { get; set; }
        public string RecipientCity { get; set; }
        public string RecipientPostCode { get; set; }
        public double RecipientLat { get; set; }
        public double RecipientLon { get; set; }

        public Package()
        {
        }

        public Package(int id, PackageSize size)
        {
            Id = id;
            Size = size;
        }
    }




}
