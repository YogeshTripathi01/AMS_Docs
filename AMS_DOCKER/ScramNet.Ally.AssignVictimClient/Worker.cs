using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mossharbor.AzureWorkArounds.ServiceBus;
using Newtonsoft.Json;
using ScramNet.Ally.AssignVictimClient.Models;

namespace ScramNet.Ally.AssignVictimClient
{
    public class Worker : BackgroundService
    {
        static ILogger<Worker> _logger;
        static ServiceBusSettings _serviceBusSettings;
        static TopicClient _topicClient;
        static ISubscriptionClient _subscriptionClient;
        static IRepository<VictimClient> _repository;


        public Worker(ILogger<Worker> logger, ServiceBusSettings serviceBusSettings, IRepository<VictimClient> repository)
        {
            _logger = logger;
            _serviceBusSettings = serviceBusSettings;
            _repository = repository;
            _topicClient = new TopicClient(serviceBusSettings.ConnectionString, serviceBusSettings.TopicName);
            _subscriptionClient = new SubscriptionClient(serviceBusSettings.ConnectionString, serviceBusSettings.TopicName, serviceBusSettings.SubscriptionName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("======================================================");
            _logger.LogInformation("Listener starting for assigning client!");
            _logger.LogInformation("======================================================");


            _logger.LogInformation("======================================================");
            _logger.LogInformation("Creating Subscription if one does not exist");
            _logger.LogInformation("======================================================");

            try
            {
                NamespaceManager ns = NamespaceManager.CreateFromConnectionString(_serviceBusSettings.ConnectionString);
                ns.CreateSubscription(_serviceBusSettings.TopicName, _serviceBusSettings.SubscriptionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            RegisterOnMessageHandlerAndReceiveMessages();

            while (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("======================================================");
                _logger.LogInformation("Listener stopping for  assigning client!");
                _logger.LogInformation("======================================================");
                await _subscriptionClient.CloseAsync();
            }
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var updatedRecord = new ReceivedClientData();
            var jsonEventFormatter = new JsonEventFormatter();

            try
            {
                var cloudMessage = jsonEventFormatter.DecodeStructuredEvent(message.Body);

                if (cloudMessage.Type == Constants.AssignClientType)
                {
                    updatedRecord = JsonConvert.DeserializeObject<ReceivedClientData>(cloudMessage.Data.ToString());
                    var victimClient = new VictimClient(updatedRecord);
                    await _repository.InsertRecord(victimClient);

                    var cloudSuccessEvent = new CloudEvent(Constants.SuccessType, new Uri("urn:" + Constants.ApplicationName))
                    {
                        DataContentType = new ContentType(MediaTypeNames.Application.Json),
                        Data = JsonConvert.SerializeObject(new SuccessBody
                        {
                            VictimId = updatedRecord.VictimId,
                            ClientId = updatedRecord.ClientId
                        })
                    };

                    var messageJson = new Message(jsonEventFormatter.EncodeStructuredEvent(cloudSuccessEvent, out var _));
                    await _topicClient.SendAsync(messageJson).ConfigureAwait(false);
                }
            }
            catch (JsonSerializationException)
            {
                _logger.LogError("Failed to deserialize event");
                _logger.LogInformation(Encoding.UTF8.GetString(message.Body));
            }
            catch (Exception ex)
            {
                var cloudFailureEvent =
                    new CloudEvent(Constants.FailureType, new Uri("urn:" + Constants.ApplicationName))
                    {
                        DataContentType = new ContentType(MediaTypeNames.Application.Json),
                        Data = JsonConvert.SerializeObject(new FailureBody
                        {
                            VictimId = updatedRecord.VictimId,
                            FailureReason = ex.Message
                        })
                    };



                var messageJson = new Message(jsonEventFormatter.EncodeStructuredEvent(cloudFailureEvent, out var _));
                await _topicClient.SendAsync(messageJson).ConfigureAwait(false);
                _logger.LogError($"Failed to update Assigned Client {updatedRecord.VictimId}");
                _logger.LogInformation(ex.Message);
            }

            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogInformation($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogInformation("Exception context for troubleshooting:");
            _logger.LogInformation($"- Endpoint: {context.Endpoint}");
            _logger.LogInformation($"- Entity Path: {context.EntityPath}");
            _logger.LogInformation($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
