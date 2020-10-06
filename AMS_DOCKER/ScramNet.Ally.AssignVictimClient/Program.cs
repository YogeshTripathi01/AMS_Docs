using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization.Conventions;
using ScramNet.Ally.AssignVictimClient.Models;

namespace ScramNet.Ally.AssignVictimClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.AddEnvironmentVariables();
            })
                .ConfigureServices((hostContext, services) =>
                {
                    var envVariables = Environment.GetEnvironmentVariables();                                    
                    
                    ServiceBusSettings serviceBusSettings = new ServiceBusSettings
                    {
                        ConnectionString = envVariables["SERVICEBUS_CONNECTIONSTRING"].ToString(),
                        SubscriptionName = envVariables["SERVICEBUS_SUBSCRIPTION"].ToString(),
                        TopicName = envVariables["SERVICEBUS_TOPIC"].ToString()
                    };

                    DatabaseSettings databaseSettings = new DatabaseSettings
                    {
                        ConnectionString = envVariables["MONGO_DATABASE_CONNECTIONSTRING"].ToString(),
                        DatabaseName = envVariables["VICTIM_DATABASENAME"].ToString(),
                        CollectionName = envVariables["COLLECTION_NAME"].ToString()
                    };

                    if (string.IsNullOrWhiteSpace(serviceBusSettings.ConnectionString)
                    || string.IsNullOrWhiteSpace(serviceBusSettings.TopicName)
                    || string.IsNullOrWhiteSpace(serviceBusSettings.SubscriptionName))
                        throw new InvalidOperationException("Can not start the service without valid service bus settings!");

                    if (string.IsNullOrWhiteSpace(databaseSettings.ConnectionString)
                    || string.IsNullOrWhiteSpace(databaseSettings.DatabaseName)
                    || string.IsNullOrWhiteSpace(databaseSettings.CollectionName))
                        throw new InvalidOperationException("Can not start the service without valid database settings!");

                    var pack = new ConventionPack();
                    pack.Add(new IgnoreExtraElementsConvention(true));
                    ConventionRegistry.Register("My Solution Conventions", pack, t => true);

                    services.AddSingleton(serviceBusSettings);
                    services.AddSingleton(databaseSettings);
                    services.AddSingleton<IRepository<VictimClient>, Repository>();
                    services.AddHostedService<Worker>();
                });
    }
}
