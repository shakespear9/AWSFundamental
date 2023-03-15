
using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;
using System.Text.Json;

// Already Authenticate to AWS by setting through AWS Cli
var sqsClient = new AmazonSQSClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "leen@nuttakorn.com",
    FullName = "Nuttakorn Tedthong",
    DateOfBirth = new DateTime(1998, 1, 1),
    GitHubUsername = "shakespear9",
};

//Get SQS Url From SQS's service name
var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            "MessageType", new MessageAttributeValue
            {
                DataType = "String",
                StringValue = nameof(CustomerCreated)
            }
        }
    }
};


var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.WriteLine();
