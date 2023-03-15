﻿using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Api.Messaging;

public class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly IOptions<TopicSetting> _topicSettings;
    private string? _topicArn;

    public SnsMessenger(IAmazonSimpleNotificationService sns, IOptions<TopicSetting> topicSettings)
    {
        _sns = sns;
        _topicSettings = topicSettings;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message)
    {
        var topicArn = await GetTopicArnAsync();

        var sendMessageRequest = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await _sns.PublishAsync(sendMessageRequest);
    }

    private async ValueTask<string> GetTopicArnAsync()
    {
        if (_topicArn is not null)
        {
            return _topicArn;
        }

        var topicArnResponse = await _sns.FindTopicAsync(_topicSettings.Value.Name);
        _topicArn = topicArnResponse.TopicArn;
        return _topicArn;
    }
}
