using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.CSharp.RuntimeBinder;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageConsumerBackgroundService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageConsumerBackgroundServiceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureQueueStorage.AzureQueueStorageConsumerBackgroundService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureQueueStorageConsumerBackgroundServiceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHosting(outputTarget));
            AddUsing("System.Linq");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AzureQueueStorageBackgroundService", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.Extensions.Hosting.BackgroundService"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType("System.Collections.Generic.IEnumerable")}<{this.GetAzureQueueStorageConsumerInterfaceName()}>", "consumers", @param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "ExecuteAsync", mth =>
                    {
                        mth.Protected().Override();
                        mth.AddParameter(UseType("System.Threading.CancellationToken"), "stoppingToken");

                        var taskRun = new CSharpInvocationStatement("Task.Run")
                        .AddArgument("() => x.ConsumeAsync(stoppingToken)");


                        mth.AddReturn(new CSharpInvocationStatement("Task.WhenAll")
                            .AddArgument(new CSharpInvocationStatement("_consumers.Select")
                                .WithoutSemicolon()
                                .AddLambdaBlock("x", lambda =>
                                {
                                    var taskRunInvoc = new CSharpInvocationStatement("Task.Run")
                                        .AddArgument(new CSharpLambdaBlock("()")
                                            .WithExpressionBody("x.ConsumeAsync(stoppingToken)"))
                                        .AddArgument("stoppingToken")
                                        .WithoutSemicolon();

                                    lambda.WithExpressionBody(taskRunInvoc);
                                })
                            ).WithoutSemicolon());
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            var totalSubscribedMessages =
                ExecutionContext.MetadataManager
                    .GetExplicitlySubscribedToMessageModels(OutputTarget.Application)
                    .Count +
                ExecutionContext.MetadataManager
                    .Eventing(ExecutionContext.GetApplicationConfig().Id)
                    .GetApplicationModels().SelectMany(x => x.SubscribedMessages())
                    .Select(x => x.TypeReference.Element.AsMessageModel()).Count() +
                ExecutionContext.MetadataManager
                        .GetExplicitlySubscribedToIntegrationCommandModels(OutputTarget.Application).Count;

            return base.CanRunTemplate() && totalSubscribedMessages > 0;
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