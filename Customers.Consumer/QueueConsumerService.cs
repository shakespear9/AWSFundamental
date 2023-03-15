using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Api.Messaging;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Consumer
{
    public class QueueConsumerService : BackgroundService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<QueueSettings> _queueSettings;
        private readonly ILogger<QueueConsumerService> _logger;
        private readonly IMediator _mediator;

        public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, ILogger<QueueConsumerService> logger, IMediator mediator)
        {
            _sqs = sqs;
            _queueSettings = queueSettings;
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Get SQS Url From SQS's service name
            var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,

                // MessageAttribute not include by default so we have to explicitly specify them
                AttributeNames = new List<string> { "All" },
                MessageAttributeNames = new List<string> { "All" },
                MaxNumberOfMessages = 1

            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                foreach (var message in response.Messages)
                {

                    var messageType = message.MessageAttributes["MessageType"].StringValue;
                    var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");

                    if (type is null)
                    {
                        _logger.LogWarning("Unknown message type: {MessageType}", type);
                        continue;
                    }

                    var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                    try
                    {
                        await _mediator.Send(typedMessage, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Message failed during processing");
                        continue;
                    }

                    await _sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);

                }

                await Task.Delay(1000);
            }


        }
    }
}
