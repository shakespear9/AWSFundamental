
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SNSPublisher;
using System.Text.Json;

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "leen@nuttakorn.com",
    FullName = "Nuttakorn Tedthong",
    DateOfBirth = new DateTime(1998, 1, 1),
    GitHubUsername = "shakespear9",
};

// Already Authenticate to AWS by setting through AWS Cli
var snsClient = new AmazonSimpleNotificationServiceClient();

var topicArnResponse = await snsClient.FindTopicAsync("customers");

var publishRequest = new PublishRequest
{
    TopicArn = topicArnResponse.TopicArn,
    Message = JsonSerializer.Serialize(customer),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            "MessageType", new MessageAttributeValue()
            {
                DataType="String",
                StringValue=nameof(CustomerCreated)
            }
        }
    }
};

await snsClient.PublishAsync(publishRequest);