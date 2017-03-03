using Messaging.Core.Interfaces;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Infrastructure.Services
{
    public class AzureQueueAccessor : IQueueAccessor
    {
        private QueueClient queueClient;
        private const string ServiceBusConnectionString = "{connectionstring}";
        private const string QueueName = "widgets";

        public void HandleAndDeleteMessage(Action<string> messageHandler)
        {
            queueClient = GetClient();

            // Receive the next message from the queue (this interface should have an async version)
            var task = queueClient.ReceiveAsync();
            task.Wait();
            var message = task.Result;

            messageHandler.Invoke(message.GetBody<string>());

            message.CompleteAsync().Wait();

            queueClient.Close();
        }

        public IEnumerable<string> ListQueueUrls()
        {
            return new string[] { QueueName };
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
