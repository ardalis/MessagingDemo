using System;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Messaging.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Messaging.Infrastructure.Services
{
    public class AmazonNotificationService : ISendNotifications
    {
        private readonly IConfiguration _configuration;
        public AmazonNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetTopicARN(string id)
        {
            var client = GetClient();
            var topics = client.ListTopicsAsync().Result;
            return topics.Topics.Single(t => t.TopicArn.Contains(id)).TopicArn;
        }

        public void SendNotification(string topic, string message, string subject)
        {
            var client = GetClient();

            var request = new PublishRequest(topic, message, "TEST: Widget Created");
            var response = client.PublishAsync(request).Result;

        }

        public void ListTopics()
        {
            var client = GetClient();

            var topics = client.ListTopicsAsync().Result;

            foreach (var topic in topics.Topics)
            {
                Console.WriteLine($"TopicARN: {topic.TopicArn}");
            }
        }

        private IAmazonSimpleNotificationService GetClient()
        {
            var options = _configuration.GetAWSOptions();

            return options.CreateServiceClient<IAmazonSimpleNotificationService>();
        }
    }
}
