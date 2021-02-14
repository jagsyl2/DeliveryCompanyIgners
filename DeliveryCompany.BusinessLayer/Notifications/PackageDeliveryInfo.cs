﻿using System;

namespace DeliveryCompany.BusinessLayer.Notifications
{
    public class PackageDeliveryInfo
    {
        public Guid Number { get; set; }

        public string RecipientName { get; set; }
        public string RecipientSurname { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientStreet { get; set; }
        public string RecipientStreetNumber { get; set; }
        public string RecipientPostCode { get; set; }
        public string RecipientCity { get; set; }

        public string SenderEmail { get; set; }
    }
}