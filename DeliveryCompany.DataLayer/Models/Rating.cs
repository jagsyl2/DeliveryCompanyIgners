﻿using System;

namespace DeliveryCompany.DataLayer.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateTime { get; set; }
        public double CouriersRating { get; set; }
    }
}
