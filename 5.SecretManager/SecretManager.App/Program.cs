

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

var secretManagerClient = new AmazonSecretsManagerClient();

var listSecretVersionRequest = new ListSecretVersionIdsRequest
{
    SecretId = "ApiKey",
    IncludeDeprecated = true,
};

var versionResponse = await secretManagerClient.ListSecretVersionIdsAsync(listSecretVersionRequest);

var request = new GetSecretValueRequest
{
    SecretId = "ApiKey",
    //VersionId = versionResponse.Versions[0].VersionId,
    //VersionStage = "AWSCURRENT"
};

var response = await secretManagerClient.GetSecretValueAsync(request);

Console.WriteLine(response.SecretString);