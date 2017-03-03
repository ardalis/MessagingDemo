using System;
using Messaging.Core.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Message.Infrastructure.Services
{
    public class AzureNotificationService : ISendNotifications
    {
        private QueueClient queueClient;
        private const string ServiceBusConnectionString = "{connectionstring}";
        private const string QueueName = "widgets";

        public string GetTopicARN(string id)
        {
            return QueueName;
        }

        public void SendNotification(string topic, string message, string subject)
        {
            queueClient = GetClient();

            // Create a new brokered message to send to the queue
            var brokeredMessage = new BrokeredMessage(message);

            // Send the message to the queue
            queueClient.SendAsync(brokeredMessage).Wait();

            queueClient.Close();


        }

        public void ListTopics()
        {
            throw new NotImplementedException();
        }

        private QueueClient GetClient()
        {
            // Creates a ServiceBusConnectionStringBuilder object from the connection string, and sets the EntityPath.
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(ServiceBusConnectionString)
            {
                EntityPath = QueueName
            };

            // Initializes the static QueueClient variable that will be used in the ReceiveMessages method.
            queueClient = QueueClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            return queueClient;
        }
    }
}
