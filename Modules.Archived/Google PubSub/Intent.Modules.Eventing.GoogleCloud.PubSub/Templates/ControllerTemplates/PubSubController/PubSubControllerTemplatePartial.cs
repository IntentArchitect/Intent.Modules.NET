using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ControllerTemplates.PubSubController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PubSubControllerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.GoogleCloud.PubSub.ControllerTemplates.PubSubController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PubSubControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"PubSubController")
                .OnBuild(file =>
                {
                    file.AddUsing("System");
                    file.AddUsing("System.Collections.Generic");
                    file.AddUsing("System.Threading");
                    file.AddUsing("System.Threading.Tasks");
                    file.AddUsing("Google.Apis.Auth");
                    file.AddUsing("Google.Protobuf");
                    file.AddUsing("Google.Cloud.PubSub.V1");
                    file.AddUsing("Microsoft.AspNetCore.Mvc");
                    file.AddUsing("Microsoft.Extensions.Options");
                    file.AddUsing("Newtonsoft.Json");
                    file.AddUsing("ProtoTimestamp = Google.Protobuf.WellKnownTypes.Timestamp");

                    var priClass = file.Classes.First();
                    priClass.AddAttribute($@"Route(""api/[controller]"")");
                    priClass.WithBaseType("Controller");
                    priClass.AddField(this.GetPubSubOptionsName(), "_pubSubOptions", field => field.PrivateReadOnly());
                    priClass.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetPubSubOptionsName()}>", "pubSubOptions");
                        ctor.AddStatement("_pubSubOptions = pubSubOptions.Value;");
                        ctor.AddParameter(this.GetEventBusSubscriptionManagerInterfaceName(), "busSubscriptionManager", parm => parm.IntroduceReadonlyField());
                        ctor.AddParameter("IServiceProvider", "serviceProvider", parm => parm.IntroduceReadonlyField());
                    });
                    priClass.AddMethod("Task<IActionResult>", "PushNotification", method =>
                    {
                        method.Async();
                        method.AddAttribute($@"HttpPost(""PushNotification"")");
                        method.AddParameter("PushBody", "body", parm => parm.AddAttribute("FromBody"));
                        method.AddParameter("string", "token", parm => parm.AddAttribute("FromQuery"));
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatementBlock("if (body == null || body.message == null)", block =>
                        {
                            block.AddStatement("return BadRequest();");
                        });
                        method.AddStatement(string.Empty);
                        method.AddStatementBlock("if (_pubSubOptions.ShouldAuthorizePushNotification)", block =>
                        {
                            block.AddStatement(@"string authorizaionHeader = HttpContext.Request.Headers[""Authorization""];")
                                .AddStatement(@"string verificationToken = token ?? body.message.attributes[""token""];")
                                .AddStatement(@"string authToken = authorizaionHeader.StartsWith(""Bearer "") ? authorizaionHeader.Substring(7) : null;")
                                .AddStatementBlock("if ((_pubSubOptions.HasVerificationToken() && verificationToken != _pubSubOptions.VerificationToken) || authToken is null)",
                                    block2 =>
                                    {
                                        block2.AddStatement("return new BadRequestResult();");
                                    })
                                .AddStatement(string.Empty)
                                .AddStatement("var payload = await JsonWebSignature.VerifySignedTokenAsync<PubSubPayload>(authToken, cancellationToken: cancellationToken);");
                        });
                        method.AddStatement(string.Empty);
                        method.AddStatement("var message = new PubsubMessage();");
                        method.AddStatement("message.Attributes.Add(body.message.attributes);");
                        method.AddStatement("message.MessageId = body.message.message_id ?? body.message.messageId;");
                        method.AddStatement("message.PublishTime = !string.IsNullOrWhiteSpace(body.message.publish_time) ? ProtoTimestamp.FromDateTime(DateTime.Parse(body.message.publish_time)) : null;");
                        method.AddStatement("message.Data = ByteString.FromBase64(body.message.data);");
                        method.AddStatement(string.Empty);
                        method.AddStatement("await RequestHandler(message, cancellationToken);");
                        method.AddStatement(string.Empty);
                        method.AddStatement("return Ok();");
                    });
                    priClass.AddMethod("Task", "RequestHandler", method =>
                    {
                        method.Async();
                        method.Private();
                        method.AddParameter("PubsubMessage", "message");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("await _busSubscriptionManager.DispatchAsync(_serviceProvider, message, cancellationToken);");
                    });
                    priClass.AddNestedClass("PushBody", nested =>
                    {
                        nested.WithComments(@"
/// <summary>
/// Pubsub messages will arrive in this format.
/// </summary>");
                        nested.AddProperty("PushMessage", "message");
                        nested.AddProperty("string", "subscription");
                    });
                    // See here for sample: https://cloud.google.com/pubsub/docs/push#receive_push
                    priClass.AddNestedClass("PushMessage", nested =>
                    {
                        nested.AddProperty("Dictionary<string, string>", "attributes");
                        nested.AddProperty("string", "data");
                        nested.AddProperty("string", "message_id");
                        nested.AddProperty("string", "messageId");
                        nested.AddProperty("string", "publish_time");
                    });
                    priClass.AddNestedClass("PubSubPayload", nested =>
                    {
                        nested.WithComments(@"
/// <summary>
/// Extended JWT payload to match the pubsub payload format.
/// </summary>");
                        nested.WithBaseType("JsonWebSignature.Payload");
                        nested.AddProperty("string", "Email", prop => prop.AddAttribute(@"JsonProperty(""email"")"));
                        nested.AddProperty("string", "EmailVerified", prop => prop.AddAttribute(@"JsonProperty(""email_verified"")"));
                    });
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface), "eventBus",
                        p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                    var requestHandlerMethod = @class.FindMethod("RequestHandler");
                    requestHandlerMethod.Statements.Last().InsertBelow("await _eventBus.FlushAllAsync(cancellationToken);");
                }, order: -100);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}