using Message.Infrastructure.Services;
using Messaging.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace Consumer1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer1 Starting...");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            // Amazon
            //var app = new Consumer(config, new AmazonQueueAccessor(config));

            // Amazon
            var app = new Consumer(config, new AzureQueueAccessor());

            app.Execute();
        }
    }
}