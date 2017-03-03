using Message.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Service1
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", 
                    optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            // Use Amazon
            //var app = new Producer(config, new AmazonNotificationService(config));

            // Use Azure
            var app = new Producer(config, new AzureNotificationService());

            app.Execute();
        }
    }
}