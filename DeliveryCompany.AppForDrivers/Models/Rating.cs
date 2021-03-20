using System;

namespace DeliveryCompany.AppForDrivers.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateTime { get; set; }
        public int CouriersRating { get; set; }
    }
}
