using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();

// Already Authenticate to AWS by setting through AWS Cli
var sqsClient = new AmazonSQSClient();


//Get SQS Url From SQS's service name
var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,

    // MessageAttribute not include by default so we have to explicitly specify them
    AttributeNames = new List<string> { "All" },
    MessageAttributeNames = new List<string> { "All" }

};

while (!cts.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cts.Token);

    foreach (var message in response.Messages)
    {
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");

        // Delete Message in Queue
        await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle);
    }

    await Task.Delay(3000);
}