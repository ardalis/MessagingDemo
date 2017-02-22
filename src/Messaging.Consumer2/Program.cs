using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;

namespace Service2
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Service3 Starting... Inventory Management");

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var app = new Consumer(builder.Build());

            app.Execute();
        }

        public class Consumer
        {
            public IConfiguration Configuration { get; }
            public string _queueUrl = "https://sqs.us-east-2.amazonaws.com/271828920772/widgets-inventory";

            public Consumer(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public void Execute()
            {
                var options = Configuration.GetAWSOptions();
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
                        // Console.WriteLine($"Detected {response.Messages.Count} message to process.");
                        foreach (var msg in response.Messages)
                        {
                            dynamic foo = JsonConvert.DeserializeObject(msg.Body);
                            Console.WriteLine($"Tracking Inventory For: {foo.Message}!");
                            // sleep to represent time spent working on this message

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
                //Client -> WebAPI -> SNS & OK -> SQS -> Service -> SignalR


                // Client               Web Server                  AWS         Image Processor
                // Rotate Img ->        API Call ->           Send SNS
                //            <-    Return OK
                //                                            (work request) <- Read from QUEUE
                //                                                                 Rotate Image
                //                                                         <- Send SNS Work Completed
                //                         Read from Queue -> (work done)
                //                       SignalR
                // Update UI  <- Msg from SignalR

                // Updates to POC
                // 1. Clean up to extract infrastructure
                // 2. Replace AWS with Azure ServiceBus
                // 3. Track timing of messages from creation to handling
                // 4. Demo going between AWS and Azure and impact on timing

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