using DeliveryCompany.DataLayer.Models;
using EventStore.Client;
using System.Text.Json;

namespace DeliveryCompany.BusinessLayer.Notifications
{
    public interface INotificationService
    {
        void NotifyOfPackageDelivery(Package package);
    }

    public class NotificationService : INotificationService
    {
        public void NotifyOfPackageDelivery(Package package)
        {
            const string stream = "Package-delivery-stream";

            var settings = EventStoreClientSettings
                .Create("esdb://127.0.0.1:2113?tls=false");

            var packageDeliveryInfo = new PackageDeliveryInfo
            {
                Number = package.Number,
                
                RecipientName = package.RecipientName,
                RecipientEmail = package.RecipientEmail,
                RecipientSurname = package.RecipientSurname,
                RecipientStreet = package.RecipientStreet,
                RecipientStreetNumber = package.RecipientStreetNumber,
                RecipientPostCode = package.RecipientPostCode,
                RecipientCity = package.RecipientCity,
                
                SenderEmail = package.Sender.Email,
            };

            using (var client = new EventStoreClient(settings))
            {
                client.AppendToStreamAsync(
                    stream,
                    StreamState.Any,
                    new[] { GetEventDataFor(packageDeliveryInfo)}).Wait();
            }
        }

        private static EventData GetEventDataFor(PackageDeliveryInfo packageDeliveryInfo)
        {
            return new EventData(
                Uuid.NewUuid(),
                "Package-delivery-arrival",
                JsonSerializer.SerializeToUtf8Bytes(packageDeliveryInfo));
        }
    }
}
