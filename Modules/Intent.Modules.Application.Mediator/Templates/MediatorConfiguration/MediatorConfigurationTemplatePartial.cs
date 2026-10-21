using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Mediator.Templates.AuthorizationBehaviour;
using Intent.Modules.Application.Mediator.Templates.LoggingBehaviour;
using Intent.Modules.Application.Mediator.Templates.MessageBusPublishBehaviour;
using Intent.Modules.Application.Mediator.Templates.PerformanceBehaviour;
using Intent.Modules.Application.Mediator.Templates.SequentialNotificationPublisher;
using Intent.Modules.Application.Mediator.Templates.UnhandledExceptionBehaviour;
using Intent.Modules.Application.Mediator.Templates.UnitOfWorkBehaviour;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates.MediatorConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MediatorConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.MediatorConfiguration";

        public const string ConfigMetadataKey = "mediator-config";
        public const string PipelineMetadataKey = "mediator-pipeline";
        public const string BehaviourMetadataKey = "mediator-behaviour";
        public const string FlushMetadataKey = "eventbus-flush";

        private const string ValidationBehaviourRole = "Infrastructure.Mediator.Dispatch.Behaviours.ValidationBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MediatorConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MediatorSourceGenerator(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Mediator")
                .AddClass("MediatorConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "AddMediatorConfiguration", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        method.AddInvocationStatement("services.AddMediator", invocation =>
                        {
                            invocation.AddMetadata(ConfigMetadataKey, true);
                            invocation.AddArgument(new CSharpLambdaBlock("(MediatorOptions options)"), argument =>
                            {
                                var options = (CSharpLambdaBlock)argument;
                                var pipeline = new CSharpCollectionExpression(null).WithItemsOnNewLines(true);
                                invocation.AddMetadata(PipelineMetadataKey, pipeline);

                                options.AddStatement("options.ServiceLifetime = ServiceLifetime.Transient;");
                                options.AddStatement($"options.Assemblies = [typeof({GetApplicationAssemblyMarkerName()})];");
                                options.AddStatement($"options.NotificationPublisherType = typeof({GetTypeName(SequentialNotificationPublisherTemplate.TemplateId)});");
                                options.AddStatement(new CSharpAssignmentStatement("options.PipelineBehaviors", pipeline));

                                CSharpFile.OnBuild(_ =>
                                {
                                    foreach (var behaviour in GetPipelineBehaviours())
                                    {
                                        pipeline.AddItem($"typeof({behaviour.TypeName}<,>)", item =>
                                        {
                                            item.AddMetadata(BehaviourMetadataKey, behaviour.Name);
                                            if (behaviour.IsFlush)
                                            {
                                                item.AddMetadata(FlushMetadataKey, true);
                                            }
                                        });
                                    }
                                });
                            });
                        });
                        method.AddReturn("services");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(extensionMethodName: "AddMediatorConfiguration")
                .ForConcern("Infrastructure")
                .HasDependency(this));
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

        private string GetApplicationAssemblyMarkerName()
        {
            return GetTypeName("Intent.Application.DependencyInjection.DependencyInjection");
        }

        private IEnumerable<PipelineBehaviour> GetPipelineBehaviours()
        {
            // The order is fixed, outermost first: the source generator reads the array as a literal,
            // so nothing else may insert, reorder or compute these items.
            var candidates = new[]
            {
                new PipelineBehaviour("LoggingBehaviour", LoggingBehaviourTemplate.TemplateId),
                new PipelineBehaviour("UnhandledExceptionBehaviour", UnhandledExceptionBehaviourTemplate.TemplateId),
                new PipelineBehaviour("PerformanceBehaviour", PerformanceBehaviourTemplate.TemplateId),
                new PipelineBehaviour("AuthorizationBehaviour", AuthorizationBehaviourTemplate.TemplateId),
                new PipelineBehaviour("MessageBusPublishBehaviour", MessageBusPublishBehaviourTemplate.TemplateId, IsFlush: true),
                new PipelineBehaviour("ValidationBehaviour", ValidationBehaviourRole),
                new PipelineBehaviour("UnitOfWorkBehaviour", UnitOfWorkBehaviourTemplate.TemplateId)
            };

            foreach (var candidate in candidates)
            {
                // GetTypeName alone is not enough: the behaviours of this always-installed module resolve
                // their type name even when their own CanRunTemplate() would skip them.
                var template = ExecutionContext.FindTemplateInstance<IIntentTemplate>(candidate.TemplateIdOrRole);
                if (template == null || !template.CanRunTemplate())
                {
                    continue;
                }

                if (TryGetTypeName(candidate.TemplateIdOrRole, out var typeName))
                {
                    yield return candidate with { TypeName = typeName };
                }
            }
        }

        private sealed record PipelineBehaviour(string Name, string TemplateIdOrRole, bool IsFlush = false)
        {
            public string TypeName { get; init; } = string.Empty;
        }
    }
}
