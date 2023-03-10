//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:7.0.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.EventingTemplates.GoogleCloudResourceManager {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    
    public partial class GoogleCloudResourceManagerTemplate : CSharpTemplateBase<object> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 10 ""
            this.Write(@"using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace ");
            
            #line default
            #line hidden
            
            #line 21 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Namespace ));
            
            #line default
            #line hidden
            
            #line 21 ""
            this.Write(";\r\n\r\npublic class ");
            
            #line default
            #line hidden
            
            #line 23 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 23 ""
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 23 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetCloudResourceManagerInterfaceName() ));
            
            #line default
            #line hidden
            
            #line 23 ""
            this.Write("\r\n{\r\n    private readonly ILogger<");
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write("> _logger;\r\n    private readonly ");
            
            #line default
            #line hidden
            
            #line 26 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetPubSubOptionsName() ));
            
            #line default
            #line hidden
            
            #line 26 ""
            this.Write(" _pubSubOptions;\r\n    private readonly HttpClient _httpClient;\r\n    \r\n    public " +
                    "");
            
            #line default
            #line hidden
            
            #line 29 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 29 ""
            this.Write("(IOptions<");
            
            #line default
            #line hidden
            
            #line 29 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetPubSubOptionsName() ));
            
            #line default
            #line hidden
            
            #line 29 ""
            this.Write("> pubSubOptions, ILogger<");
            
            #line default
            #line hidden
            
            #line 29 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 29 ""
            this.Write("> logger)\r\n    {\r\n        _logger = logger;\r\n        _pubSubOptions = pubSubOptio" +
                    "ns.Value;\r\n        _httpClient = new HttpClient();\r\n        _httpClient.DefaultR" +
                    "equestHeaders.Add(\"Metadata-Flavor\", \"Google\");\r\n        PopulateDetails();\r\n   " +
                    " }\r\n\r\n    public string ProjectId { get; private set; }\r\n    public bool ShouldS" +
                    "etupCloudResources { get; private set; }\r\n\r\n    public async Task CreateTopicIfN" +
                    "otExistAsync(string topicId, CancellationToken cancellationToken = default)\r\n   " +
                    " {\r\n        var publisherService = await new PublisherServiceApiClientBuilder\r\n " +
                    "       {\r\n            EmulatorDetection = _pubSubOptions.GetEmulatorDetectionMod" +
                    "e()\r\n        }.BuildAsync(cancellationToken);\r\n        try\r\n        {\r\n         " +
                    "   var topic = await publisherService.GetTopicAsync($\"projects/{ProjectId}/topic" +
                    "s/{topicId}\");\r\n            return;\r\n        }\r\n        catch (RpcException ex)\r" +
                    "\n        {\r\n            if (ex.Status.StatusCode != StatusCode.NotFound)\r\n      " +
                    "      {\r\n                throw;\r\n            }\r\n        }\r\n\r\n        try\r\n      " +
                    "  {\r\n            _logger.LogInformation($\"Creating Topic {topicId} for Project {" +
                    "ProjectId} on Google Cloud Pub/Sub...\");\r\n            var newTopic = await publi" +
                    "sherService.CreateTopicAsync(\r\n                name: new TopicName(ProjectId, to" +
                    "picId), \r\n                cancellationToken);\r\n        }\r\n        catch (RpcExce" +
                    "ption ex)\r\n        {\r\n            if (ex.Status.StatusCode != StatusCode.Already" +
                    "Exists)\r\n            {\r\n                throw;\r\n            }\r\n        }\r\n    }\r" +
                    "\n\r\n    public async Task CreateSubscriptionIfNotExistAsync((string SubscriptionI" +
                    "d, string TopicId) subscription, CancellationToken cancellationToken = default)\r" +
                    "\n    {\r\n        var subscriberService = await new SubscriberServiceApiClientBuil" +
                    "der()\r\n        {\r\n            EmulatorDetection = _pubSubOptions.GetEmulatorDete" +
                    "ctionMode()\r\n        }.BuildAsync(cancellationToken);\r\n        try\r\n        {\r\n " +
                    "           var existingSubscription = await subscriberService.GetSubscriptionAsy" +
                    "nc($\"projects/{ProjectId}/subscriptions/{subscription.SubscriptionId}\", cancella" +
                    "tionToken);\r\n            return;\r\n        }\r\n        catch (RpcException ex)\r\n  " +
                    "      {\r\n            if (ex.Status.StatusCode != StatusCode.NotFound)\r\n         " +
                    "   {\r\n                throw;\r\n            }\r\n        }\r\n\r\n        try\r\n        {" +
                    "\r\n            _logger.LogInformation($\"Creating Subscription {subscription.Subsc" +
                    "riptionId} for Topic {subscription.TopicId} in Project {ProjectId} on Google Clo" +
                    "ud Pub/Sub...\");\r\n            var newSubscription = await subscriberService.Crea" +
                    "teSubscriptionAsync(\r\n                name: new SubscriptionName(ProjectId, subs" +
                    "cription.SubscriptionId),\r\n                topic: new TopicName(ProjectId, subsc" +
                    "ription.TopicId),\r\n                pushConfig: null,\r\n                ackDeadlin" +
                    "eSeconds: 60,\r\n                cancellationToken: cancellationToken);\r\n        }" +
                    "\r\n        catch (RpcException nestedEx)\r\n        {\r\n            if (nestedEx.Sta" +
                    "tus.StatusCode != StatusCode.AlreadyExists)\r\n            {\r\n                thro" +
                    "w;\r\n            }\r\n        }\r\n    }\r\n\r\n    private void PopulateDetails()\r\n    {" +
                    "\r\n        ProjectId = _pubSubOptions.UseMetadataServer ? GetGoogleProjectId() : " +
                    "_pubSubOptions.ProjectId;\r\n\r\n        if (ProjectId == null)\r\n        {\r\n        " +
                    "    throw new Exception(@\"No Project Id has been specified for use with Google C" +
                    "loud Pub/Sub.\r\nEither configure \'GoogleCloud:UseMetadataServer\' to \'true\' to use" +
                    " the Metadata server or configure \'GoogleCloud:ProjectId\' to be the actual Proje" +
                    "ct Id in your appsettings.json file.\");\r\n        }\r\n\r\n        ShouldSetupCloudRe" +
                    "sources = _pubSubOptions.ShouldSetupCloudResources;\r\n    }\r\n\r\n    private string" +
                    " GetGoogleProjectId()\r\n    {\r\n        var request = new HttpRequestMessage() {\r\n" +
                    "            RequestUri = new Uri(\"http://metadata.google.internal/computeMetadat" +
                    "a/v1/project/project-id\"),\r\n            Method = HttpMethod.Get\r\n        };\r\n   " +
                    "     return _httpClient.Send(request).Content.ReadAsStringAsync().Result;\r\n    }" +
                    "\r\n}");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}
