using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using OutboxPatternType = Intent.Modules.Eventing.MassTransit.Settings.EventingSettings.OutboxPatternOptionsEnum;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventConsumer;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class IntegrationEventConsumerTemplate : CSharpTemplateBase<object, ConsumerDecorator>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventConsumer";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public IntegrationEventConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NuGetPackages.MassTransitAbstractions);
        AddTypeSource(IntegrationEventMessageTemplate.TemplateId);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddUsing("System.Threading.Tasks")
            .AddUsing("MassTransit")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass($"IntegrationEventConsumer", @class =>
            {
                @class.AddGenericParameter("THandler", out var tHandler);
                @class.AddGenericParameter("TMessage", out var tMessage);
                @class.ImplementsInterface($"IConsumer<{tMessage}>");
                @class.AddGenericTypeConstraint(tMessage, g => g.AddType("class"));
                @class.AddGenericTypeConstraint(tHandler,
                    g => g.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));

                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter("IServiceProvider", "serviceProvider", parm => parm.IntroduceReadonlyField());
                });

                @class.AddMethod("Task", "Consume", method =>
                {
                    method.Async();
                    method.AddParameter($"ConsumeContext<{tMessage}>", "context");
                    method.AddStatement(
                        $"var eventBus = _serviceProvider.GetService<{this.GetMassTransitEventBusName()}>(){(UseExplicitNullSymbol ? "!" : string.Empty)};");
                    method.AddStatement($"eventBus.ConsumeContext = context;");
                    method.AddStatement(
                        $"var handler = _serviceProvider.GetService<{tHandler}>(){(UseExplicitNullSymbol ? "!" : string.Empty)};",
                        stmt => stmt.AddMetadata("handler", "instantiate"));
                    method.AddStatement($"await handler.HandleAsync(context.Message, context.CancellationToken);",
                        stmt => stmt.AddMetadata("handler", "execute"));
                    method.AddStatement($"await eventBus.FlushAllAsync(context.CancellationToken);",
                        stmt => stmt.AddMetadata("event-bus-flush", true));

                    if (this.SystemUsesPersistenceUnitOfWork())
                    {
                        ApplyUnitOfWorkSaves(@class, method);
                    }
                });
            })
            .AddClass($"IntegrationEventConsumerDefinition", @class =>
            {
                @class.AddGenericParameter("THandler", out var tHandler);
                @class.AddGenericParameter("TMessage", out var tMessage);
                @class.WithBaseType($"ConsumerDefinition<IntegrationEventConsumer<{tHandler}, {tMessage}>>");
                @class.AddGenericTypeConstraint(tMessage, g => g.AddType("class"));
                @class.AddGenericTypeConstraint(tHandler, g => g.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));
                @class.AddMethod("void", "ConfigureConsumer", method =>
                {
                    method.Protected().Override();
                    method.AddParameter("IReceiveEndpointConfigurator", "endpointConfigurator");
                    method.AddParameter($"IConsumerConfigurator<IntegrationEventConsumer<{tHandler}, {tMessage}>>", "consumerConfigurator");
                    method.AddParameter("IRegistrationContext", "context");

                    switch (ExecutionContext.Settings.GetEventingSettings().OutboxPattern().AsEnum())
                    {
                        case OutboxPatternType.InMemory:
                            method.AddStatement("endpointConfigurator.UseInMemoryInboxOutbox(context);");
                            break;
                        case OutboxPatternType.EntityFramework when EfIsPresent():
                            method.AddStatement($"endpointConfigurator.UseEntityFrameworkOutbox<{GetTypeName("Infrastructure.Data.DbContext")}>(context);");
                            break;
                        default:
                            // Do nothing
                            break;
                    }
                });
            });
    }

    private bool EfIsPresent()
    {
        return TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out _) ||
               TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out _);
    }

    private void ApplyUnitOfWorkSaves(CSharpClass @class, CSharpClassMethod method)
    {
        var executeHandler = method.FindStatement(x => x.TryGetMetadata("handler", out string value) && value == "execute");
        executeHandler.Remove();

        var flushAll = method.FindStatement(p => p.HasMetadata("event-bus-flush"));
        flushAll.Remove();

        var outboxPatternType = ExecutionContext.Settings.GetEventingSettings().OutboxPattern().AsEnum();

        // When we're using EF's outbox pattern, then MassTransit itself creates a transaction and saves the changes
        var allowTransactionScope = outboxPatternType != OutboxPatternType.EntityFramework;

        method.ApplyUnitOfWorkImplementations(
            template: this,
            constructor: @class.Constructors.First(),
            invocationStatement: executeHandler,
            allowTransactionScope: allowTransactionScope,
            cancellationTokenExpression: "context.CancellationToken");

        switch (outboxPatternType)
        {
            case OutboxPatternType.None:
            case OutboxPatternType.InMemory:
                method.AddStatement(flushAll);
                break;
            case OutboxPatternType.EntityFramework:
                //There is a factory extension which has already baked this into the DBContext
                /*                {
                                    //Flush Domain Events
                                    var saveStatement = method.FindStatement(p => p.TryGetMetadata("transaction", out string transaction) && transaction == "save-changes");
                                    saveStatement.InsertBelow(flushAll);
                                    //Save any Integration Events
                                    flushAll.InsertBelow(saveStatement.ToString());
                                }*/
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool UseExplicitNullSymbol => Project.GetProject().NullableEnabled;

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