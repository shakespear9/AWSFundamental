using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleDynamoDBLambda;

public class Function
{
    public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
    {
        context.Logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");

        foreach (var record in dynamoEvent.Records)
        {
            context.Logger.LogInformation($"Event ID: {record.EventID}");
            context.Logger.LogInformation($"Event Name: {record.EventName}");

            if (record.Dynamodb.OldImage != null)
            {
                var oldDocument = Document.FromAttributeMap(record.Dynamodb.OldImage).ToJsonPretty();
                context.Logger.LogInformation($"Old Document ${oldDocument}");
            }

            var newDocument = Document.FromAttributeMap(record.Dynamodb.NewImage).ToJsonPretty();
            context.Logger.LogInformation($"New Document: {newDocument}");
            // TODO: Add business logic processing the record.Dynamodb object.
        }

        context.Logger.LogInformation("Stream processing complete.");
    }
}