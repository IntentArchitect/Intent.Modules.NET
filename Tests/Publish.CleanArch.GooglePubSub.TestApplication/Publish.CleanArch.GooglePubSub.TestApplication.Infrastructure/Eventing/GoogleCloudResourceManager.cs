using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GoogleCloudResourceManager", Version = "1.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Eventing;

public class GoogleCloudResourceManager : ICloudResourceManager
{
    private readonly ILogger<GoogleCloudResourceManager> _logger;
    private readonly PubSubOptions _pubSubOptions;
    private readonly HttpClient _httpClient;

    public GoogleCloudResourceManager(IOptions<PubSubOptions> pubSubOptions, ILogger<GoogleCloudResourceManager> logger)
    {
        _logger = logger;
        _pubSubOptions = pubSubOptions.Value;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Metadata-Flavor", "Google");
        PopulateDetails();
    }

    public string ProjectId { get; private set; }
    public bool ShouldSetupCloudResources { get; private set; }

    public async Task CreateTopicIfNotExistAsync(string topicId, CancellationToken cancellationToken = default)
    {
        var publisherService = await new PublisherServiceApiClientBuilder
        {
            EmulatorDetection = _pubSubOptions.GetEmulatorDetectionMode()
        }.BuildAsync(cancellationToken);
        try
        {
            var topic = await publisherService.GetTopicAsync($"projects/{ProjectId}/topics/{topicId}");
            return;
        }
        catch (RpcException ex)
        {
            if (ex.Status.StatusCode != StatusCode.NotFound)
            {
                throw;
            }
        }

        try
        {
            _logger.LogInformation($"Creating Topic {topicId} for Project {ProjectId} on Google Cloud Pub/Sub...");
            var newTopic = await publisherService.CreateTopicAsync(
                name: new TopicName(ProjectId, topicId),
                cancellationToken);
        }
        catch (RpcException ex)
        {
            if (ex.Status.StatusCode != StatusCode.AlreadyExists)
            {
                throw;
            }
        }
    }

    public async Task CreateSubscriptionIfNotExistAsync((string SubscriptionId, string TopicId) subscription, CancellationToken cancellationToken = default)
    {
        var subscriberService = await new SubscriberServiceApiClientBuilder()
        {
            EmulatorDetection = _pubSubOptions.GetEmulatorDetectionMode()
        }.BuildAsync(cancellationToken);
        try
        {
            var existingSubscription = await subscriberService.GetSubscriptionAsync($"projects/{ProjectId}/subscriptions/{subscription.SubscriptionId}", cancellationToken);
            return;
        }
        catch (RpcException ex)
        {
            if (ex.Status.StatusCode != StatusCode.NotFound)
            {
                throw;
            }
        }

        try
        {
            _logger.LogInformation($"Creating Subscription {subscription.SubscriptionId} for Topic {subscription.TopicId} in Project {ProjectId} on Google Cloud Pub/Sub...");
            var newSubscription = await subscriberService.CreateSubscriptionAsync(
                name: new SubscriptionName(ProjectId, subscription.SubscriptionId),
                topic: new TopicName(ProjectId, subscription.TopicId),
                pushConfig: null,
                ackDeadlineSeconds: 60,
                cancellationToken: cancellationToken);
        }
        catch (RpcException nestedEx)
        {
            if (nestedEx.Status.StatusCode != StatusCode.AlreadyExists)
            {
                throw;
            }
        }
    }

    private void PopulateDetails()
    {
        ProjectId = _pubSubOptions.UseMetadataServer ? GetGoogleProjectId() : _pubSubOptions.ProjectId;

        if (ProjectId == null)
        {
            throw new Exception(@"No Project Id has been specified for use with Google Cloud Pub/Sub.
Either configure 'GoogleCloud:UseMetadataServer' to 'true' to use the Metadata server or configure 'GoogleCloud:ProjectId' to be the actual Project Id in your appsettings.json file.");
        }

        ShouldSetupCloudResources = _pubSubOptions.ShouldSetupCloudResources;
    }

    private string GetGoogleProjectId()
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri("http://metadata.google.internal/computeMetadata/v1/project/project-id"),
            Method = HttpMethod.Get
        };
        return _httpClient.Send(request).Content.ReadAsStringAsync().Result;
    }
}