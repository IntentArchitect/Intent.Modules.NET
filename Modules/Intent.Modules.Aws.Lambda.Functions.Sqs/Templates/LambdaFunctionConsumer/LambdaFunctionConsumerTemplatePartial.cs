using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Aws.Sqs.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.UnitOfWork.Shared;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

using LambdaNugetPackages = Intent.Modules.Aws.Lambda.Functions.NugetPackages;
using SqsNugetPackages = Intent.Modules.Aws.Sqs.NugetPackages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Sqs.Templates.LambdaFunctionConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class LambdaFunctionConsumerTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Lambda.Functions.Sqs.LambdaFunctionConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public LambdaFunctionConsumerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(LambdaNugetPackages.AmazonLambdaCore(OutputTarget));
            AddNugetDependency(LambdaNugetPackages.AmazonLambdaAnnotations(OutputTarget));
            AddNugetDependency(SqsNugetPackages.AmazonLambdaSqsEvents(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Amazon.Lambda.Annotations")
                .AddUsing("Amazon.Lambda.Core")
                .AddUsing("Amazon.Lambda.SQSEvents")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass(GetConsumerName(), @class =>
                {
                    @class.RepresentsModel(Model);

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"ILogger<{@class.Name}>", "logger", param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
                        ctor.AddParameter(this.GetSqsMessageDispatcherInterfaceName(), "dispatcher", param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
                        ctor.AddParameter(this.GetEventBusInterfaceName(), "eventBus", param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
                    });

                    @class.AddMethod("Task", "ProcessAsync", method =>
                    {
                        method.Async();
                        method.AddAttribute("LambdaFunction");
                        method.AddParameter(UseType("Amazon.Lambda.SQSEvents.SQSEvent"), "sqsEvent");
                        method.AddParameter(UseType("Amazon.Lambda.Core.ILambdaContext"), "context");

                        method.AddStatement("// AWSLambda0107: passing CancellationToken parameters is not supported; use CancellationToken.None instead.");
                        method.AddStatement("var cancellationToken = CancellationToken.None;");

                        method.AddForEachStatement("record", "sqsEvent.Records", loop =>
                        {
                            loop.AddTryBlock(tryBlock =>
                            {
                                var dispatch = new CSharpAwaitExpression(new CSharpInvocationStatement("_dispatcher", "DispatchAsync")
                                    .AddArgument("_serviceProvider")
                                    .AddArgument("record")
                                    .AddArgument("cancellationToken"));
                                dispatch.AddMetadata("service-dispatch-statement", true);

                                tryBlock.ApplyUnitOfWorkImplementations(
                                    template: this,
                                    constructor: @class.Constructors.First(),
                                    invocationStatement: dispatch,
                                    cancellationTokenExpression: "cancellationToken");

                                tryBlock.AddStatement("await _eventBus.FlushAllAsync(cancellationToken);");
                            });

                            loop.AddCatchBlock(UseType("System.Exception"), "ex", catchBlock =>
                            {
                                catchBlock.AddStatement($"_logger.LogError(ex, \"Error processing {GetConsumerName()} message with ID {{MessageId}}\", record.MessageId);");
                                catchBlock.AddStatement("throw;");
                            });
                        });
                    });
                });
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

        private string GetConsumerName()
        {
            return Model.Name.RemoveSuffix("Handler").EnsureSuffixedWith("Consumer");
        }
    }
}