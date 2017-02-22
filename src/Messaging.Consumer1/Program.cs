using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;

namespace Service2
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Service2 Starting...");
            var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

            var app = new Consumer(builder.Build());

            app.Execute();
        }

        public class Consumer
        {
            public IConfiguration Configuration { get; }
            //public string _queueUrl = "https://sqs.us-east-1.amazonaws.com/271828920772/Widgets";
            //public string _queueUrl = "https://sqs.us-east-2.amazonaws.com/271828920772/widgets.fifo";
            public string _queueUrl = "https://sqs.us-east-2.amazonaws.com/271828920772/widgets";

            public Consumer(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public void Execute()
            {
                var options = Configuration.GetAWSOptions();


                Console.WriteLine($"Profile: {options.Profile}");
                Console.WriteLine($"Region: {options.Region}");
                Console.WriteLine("Queues:");
                ListQueues();
                ConsumeMessages();

            }

            private IAmazonSQS GetClient()
            {
                var options = Configuration.GetAWSOptions();

                return options.CreateServiceClient<IAmazonSQS>();
            }

            public void ConsumeMessages()
            {
                var client = GetClient();

                while (true)
                {
                    var response = client.ReceiveMessageAsync(_queueUrl).Result;
                    if (response.Messages.Count > 0)
                    {
                        Console.WriteLine($"Detected {response.Messages.Count} message to process.");
                        foreach (var msg in response.Messages)
                        {
                            Console.WriteLine($"Handling message: {msg.Body}!");

                            // Delete our message so that it doesn't get handled again
                            var receiptHandle = msg.ReceiptHandle;
                            client.DeleteMessageAsync(_queueUrl, receiptHandle).Wait();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No messages.");
                    }
                    Thread.Sleep(100);
                }
            }


            public void ListQueues()
            {
                var request = new ListQueuesRequest();
                var client = GetClient();

                var response = client.ListQueuesAsync(request).Result;
                var urls = response.QueueUrls;

                if (urls.Any())
                {
                    Console.WriteLine("Queue URLs:");

                    foreach (var url in urls)
                    {
                        Console.WriteLine("  " + url);
                    }
                }
                else
                {
                    Console.WriteLine("No queues.");
                }

            }
        }

    }
}