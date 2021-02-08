namespace DeliveryCompany.DataLayer.Models
{
    public enum TypeOfUser
    {
        Driver,
        Customer
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
        public TypeOfUser Type { get; set; }
    }
}
