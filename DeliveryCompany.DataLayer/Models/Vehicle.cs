namespace DeliveryCompany.DataLayer.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public int LoadCapacity { get; set;  }
        public int DriverId { get; set; }
        public User Driver { get; set; }
    }
}
