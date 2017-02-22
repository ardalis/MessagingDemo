using Messaging.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;

namespace Service1
{
    public class Producer
    {
        public IConfiguration Configuration { get; }
        private readonly ISendNotifications _notifier;
        //public string _queueUrl = "https://sqs.us-east-1.amazonaws.com/271828920772/Widgets";
        //public string _queueUrl = "https://sqs.us-east-2.amazonaws.com/271828920772/widgets.fifo";
        public string _queueUrl = "https://sqs.us-east-2.amazonaws.com/271828920772/widgets";

        public Producer(IConfiguration configuration, ISendNotifications notifier)
        {
            Configuration = configuration;
            _notifier = notifier;
        }

        // Use SNS Fan-Out for multiple subscribers
        // https://aws.amazon.com/blogs/aws/queues-and-notifications-now-best-friends/
        public void Execute()
        {
            var options = Configuration.GetAWSOptions();


            Console.WriteLine($"Profile: {options.Profile}");
            Console.WriteLine($"Region: {options.Region}");
            Console.WriteLine("Queues:");
            //ListQueues();
            _notifier.ListTopics();
            SendNotifications();
            //SendMessages();
        }


        public void SendNotifications()
        {
            // Resource: http://stackoverflow.com/a/13016803/13729 

            string topicArn = _notifier.GetTopicARN("271828920772");

            //Console.WriteLine("TopicARN: " + topic.TopicArn);


            //string topic = "widget-created";

            // subscribe an email address
            // client.SubscribeAsync(new SubscribeRequest(topic.TopicArn, "email", "steve@kentsmiths.com")).Wait();

            while (true)
            {
                Console.Write("Publishing notification...");
                string message = "SNS: " + DateTime.Now.ToString();
                _notifier.SendNotification(topicArn, message, "TEST: Widget Created via Core");
                Console.WriteLine("Published! (press Enter to stop)");
                System.Threading.Thread.Sleep(1000);
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
        }
    }
}
