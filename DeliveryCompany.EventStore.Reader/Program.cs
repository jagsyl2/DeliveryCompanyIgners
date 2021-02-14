//using EventStore.Client;
//using System;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DeliveryCompany.EventStore.Reader
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            const string stream = "EmailNotifications";

//            var settings = EventStoreClientSettings
//                .Create("esdb://127.0.0.1:2113?tls=false");

//            using (var client = new EventStoreClient(settings))
//            {
//                client.SubscribeToStreamAsync(
//                    stream,
//                    EventArrived);
//            }
//        }

//        private static Task EventArrived(
//            StreamSubscription streamSubscription, 
//            ResolvedEvent resolvedEvent, 
//            CancellationToken cancellationToken)
//        {
//                var jsonData = Encoding.UTF8.GetString(resolvedEvent.Data.ToArray());
//                Console.WriteLine(jsonData);

//        }
//    }
//}
