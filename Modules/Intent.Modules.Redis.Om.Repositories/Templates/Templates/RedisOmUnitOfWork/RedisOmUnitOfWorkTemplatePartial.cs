using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmUnitOfWork
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmUnitOfWorkTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmUnitOfWork";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmUnitOfWorkTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"RedisOmUnitOfWork", @class =>
                {
                    TryGetTemplate<IClassProvider>("Intent.DomainEvents.DomainEventServiceInterface", out var domainEventServiceTemplate);

                    @class
                        .Internal()
                        .ImplementsInterface(this.GetRedisOmUnitOfWorkInterfaceName())
                        .AddField("ConcurrentQueue<Func<CancellationToken, Task>>", "_actions", field => field
                            .PrivateReadOnly()
                            .WithAssignment("new()")
                        )
                        .AddField("ConcurrentDictionary<object, byte>", "_trackedEntities", field => field
                            .PrivateReadOnly()
                            .WithAssignment("new()")
                        )
                    ;

                    if (domainEventServiceTemplate != null)
                    {
                        @class.AddConstructor(ctor => ctor
                            .AddParameter(GetTypeName(domainEventServiceTemplate), "domainEventService", parameter => parameter
                                .IntroduceReadonlyField()
                            )
                        );
                    }

                    @class
                        .AddMethod("void", "Track", method => method
                            .AddParameter("object?", "entity")
                            .AddIfStatement("entity is null", @if => @if.AddStatement("return;"))
                            .AddStatement("_trackedEntities.TryAdd(entity, default);")
                        )
                        .AddMethod("void", "Enqueue", method => method
                            .AddParameter("Func<CancellationToken, Task>", "action")
                            .AddStatement("_actions.Enqueue(action);")
                        )
                        .AddMethod("Task", "SaveChangesAsync", method =>
                        {
                            method.Async().AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                            if (domainEventServiceTemplate != null)
                            {
                                method.AddStatement("await DispatchEvents(cancellationToken);");
                            }

                            method
                                .AddStatementBlock("while (_actions.TryDequeue(out var action))", block => block
                                    .AddStatement("await action(cancellationToken);")
                                );
                        })
                    ;

                    if (domainEventServiceTemplate != null)
                    {
                        @class.AddMethod("Task", "DispatchEvents", method =>
                        {
                            method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                            method.Private().Async();
                            method.AddWhileStatement("true", @while => @while
                                .AddMethodChainStatement("var domainEventEntity = _trackedEntities", chain => chain
                                    .AddChainStatement("Keys")
                                    .AddChainStatement($"OfType<{GetTypeName("Intent.DomainEvents.HasDomainEventInterface")}>()")
                                    .AddChainStatement("SelectMany(x => x.DomainEvents)")
                                    .AddChainStatement("FirstOrDefault(domainEvent => !domainEvent.IsPublished)"))
                                .AddIfStatement("domainEventEntity is null", @if => @if
                                    .AddStatement("break;"))
                                .AddStatement("domainEventEntity.IsPublished = true;", s => s.SeparatedFromPrevious())
                                .AddStatement("await _domainEventService.Publish(domainEventEntity, cancellationToken);"));
                        });
                    }
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            var interfaceTemplate = Project.FindTemplateInstance<IClassProvider>(RedisOmUnitOfWorkInterfaceTemplate.TemplateId);

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime()
                .WithResolveFromContainer()
                .ForInterface(interfaceTemplate));
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