using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Google.Apis.Auth;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.AspNetCore.GooglePubSub.TestApplication.Domain.Common.Interfaces;
using Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Configuration;
using Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Eventing;
using ProtoTimestamp = Google.Protobuf.WellKnownTypes.Timestamp;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.ControllerTemplates.PubSubController", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Api.Controllers
{
    [Route("api/[controller]")]
    public class PubSubController : Controller
    {
        private readonly PubSubOptions _pubSubOptions;
        private readonly IEventBusSubscriptionManager _busSubscriptionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;

        public PubSubController(IOptions<PubSubOptions> pubSubOptions,
            IEventBusSubscriptionManager busSubscriptionManager,
            IServiceProvider serviceProvider,
            IEventBus eventBus)
        {
            _pubSubOptions = pubSubOptions.Value;
            _busSubscriptionManager = busSubscriptionManager;
            _serviceProvider = serviceProvider;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [HttpPost("PushNotification")]
        public async Task<IActionResult> PushNotification(
            [FromBody] PushBody body,
            [FromQuery] string token,
            CancellationToken cancellationToken)
        {
            if (body == null || body.message == null)
            {
                return BadRequest();
            }

            if (_pubSubOptions.ShouldAuthorizePushNotification)
            {
                string authorizaionHeader = HttpContext.Request.Headers["Authorization"];
                string verificationToken = token ?? body.message.attributes["token"];
                string authToken = authorizaionHeader.StartsWith("Bearer ") ? authorizaionHeader.Substring(7) : null;
                if ((_pubSubOptions.HasVerificationToken() && verificationToken != _pubSubOptions.VerificationToken) || authToken is null)
                {
                    return new BadRequestResult();
                }

                var payload = await JsonWebSignature.VerifySignedTokenAsync<PubSubPayload>(authToken, cancellationToken: cancellationToken);
            }

            var message = new PubsubMessage();
            message.Attributes.Add(body.message.attributes);
            message.MessageId = body.message.message_id ?? body.message.messageId;
            message.PublishTime = !string.IsNullOrWhiteSpace(body.message.publish_time) ? ProtoTimestamp.FromDateTime(DateTime.Parse(body.message.publish_time)) : null;
            message.Data = ByteString.FromBase64(body.message.data);

            await RequestHandler(message, cancellationToken);

            return Ok();
        }

        private async Task RequestHandler(PubsubMessage message, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
                await _busSubscriptionManager.DispatchAsync(_serviceProvider, message, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            await _eventBus.FlushAllAsync(cancellationToken);
        }

        /// <summary>
        /// Pubsub messages will arrive in this format.
        /// </summary>
        public class PushBody
        {
            public PushMessage message { get; set; }
            public string subscription { get; set; }
        }
        public class PushMessage
        {
            public Dictionary<string, string> attributes { get; set; }
            public string data { get; set; }
            public string message_id { get; set; }
            public string messageId { get; set; }
            public string publish_time { get; set; }
        }
        /// <summary>
        /// Extended JWT payload to match the pubsub payload format.
        /// </summary>
        public class PubSubPayload : JsonWebSignature.Payload
        {
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("email_verified")]
            public string EmailVerified { get; set; }
        }
    }
}