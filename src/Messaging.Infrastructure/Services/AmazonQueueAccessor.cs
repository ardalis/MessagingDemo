using System;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Messaging.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Message.Infrastructure.Services
{
    public class AmazonQueueAccessor : IQueueAccessor
    {
        private readonly IConfiguration _configuration;

        // TODO: Move to configuration
        private readonly string _queueUrl = "https://sqs.us-east-2.amazonaws.com/271828920772/widgets";
        public AmazonQueueAccessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IAmazonSQS GetClient()
        {
            var options = _configuration.GetAWSOptions();

            return options.CreateServiceClient<IAmazonSQS>();
        }

        public IEnumerable<string> ListQueueUrls()
        {
            var request = new ListQueuesRequest();
            var client = GetClient();

            var response = client.ListQueuesAsync(request).Result;
            return response.QueueUrls;
        }

        public void HandleAndDeleteMessage(Action<string> messageHandler)
        {
            var client = GetClient();
            var response = client.ReceiveMessageAsync(_queueUrl).Result;
            if (response.Messages.Count > 0)
            {
//                Console.WriteLine($"Detected {response.Messages.Count} message to process.");
                foreach (var msg in response.Messages)
                {
                    messageHandler(msg.Body);
//                    Console.WriteLine($"Handling message: {msg.Body}!");

                    // Delete our message so that it doesn't get handled again
                    var receiptHandle = msg.ReceiptHandle;
                    client.DeleteMessageAsync(_queueUrl, receiptHandle).Wait();
                }
            }
        }
    }
}
