namespace DeliveryCompany.AppForDrivers.Models
{
    public enum TypeOfUser
    {
        Driver = 0,
        Customer = 1
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public TypeOfUser Type { get; set; }
        public string Password { get; set; }
    }
}
