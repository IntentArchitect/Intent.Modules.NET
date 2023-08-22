using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitEventBus
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MassTransitEventBusTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.MassTransitEventBus";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MassTransitEventBusTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MassTransit")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("MassTransitEventBus", @class =>
                {
                    @class.ImplementsInterface(this.GetEventBusInterfaceName());
                    @class.AddField("List<object>", "_messagesToPublish", field => field
                        .PrivateReadOnly()
                        .WithAssignment("new List<object>()"));
                    if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                    {
                        @class.AddField("List<ScheduleEntry>", "_messagesToSchedule", field => field
                            .PrivateReadOnly()
                            .WithAssignment("new List<ScheduleEntry>()"));
                    }

                    @class.AddConstructor(ctor => { ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField()); });

                    @class.AddProperty("ConsumeContext?", "ConsumeContext");

                    @class.AddMethod("void", "Publish", method =>
                    {
                        method.AddGenericParameter("T", out var T)
                            .AddGenericTypeConstraint(T, c => c.AddType("class"))
                            .AddParameter(T, "message")
                            .AddStatement("_messagesToPublish.Add(message);");
                    });

                    if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                    {
                        @class.AddMethod("void", "SchedulePublish", method =>
                        {
                            method.AddGenericParameter("T", out var T)
                                .AddGenericTypeConstraint(T, c => c.AddType("class"))
                                .AddParameter(T, "message")
                                .AddParameter("DateTime", "scheduled")
                                .AddStatement("_messagesToSchedule.Add(ScheduleEntry.ForScheduled(message, scheduled));");
                        });

                        @class.AddMethod("void", "SchedulePublish", method =>
                        {
                            method.AddGenericParameter("T", out var T)
                                .AddGenericTypeConstraint(T, c => c.AddType("class"))
                                .AddParameter(T, "message")
                                .AddParameter("TimeSpan", "delay")
                                .AddStatement("_messagesToSchedule.Add(ScheduleEntry.ForDelay(message, delay));");
                        });
                    }

                    @class.AddMethod("Task", "FlushAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddIfStatement("ConsumeContext is not null", block =>
                        {
                            block.AddStatement("await PublishWithConsumeContext(cancellationToken);");
                        });
                        method.AddElseStatement(block =>
                        {
                            block.AddStatement("await PublishWithNormalContext(cancellationToken);");
                        });
                        method.AddStatement("_messagesToPublish.Clear();", s => s.SeparatedFromPrevious());
                        if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                        {
                            method.AddStatement("_messagesToSchedule.Clear();");
                        }
                    });

                    @class.AddMethod("Task", "PublishWithConsumeContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("await ConsumeContext!.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);");

                        if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                        {
                            method.AddForEachStatement("scheduleEntry", "_messagesToSchedule", loop =>
                            {
                                loop.SeparatedFromPrevious();
                                loop.AddStatement(
                                    "await ConsumeContext!.SchedulePublish(scheduleEntry.Scheduled, scheduleEntry.Message, cancellationToken).ConfigureAwait(false);");
                            });
                        }
                    });

                    @class.AddMethod("Task", "PublishWithNormalContext", method =>
                    {
                        method.Async().Private();
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();");

                        if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                        {
                            method.AddStatement("var messageScheduler = _serviceProvider.GetRequiredService<IMessageScheduler>();");
                        }

                        method.AddStatement("await publishEndpoint.PublishBatch(_messagesToPublish, cancellationToken).ConfigureAwait(false);",
                            s => s.SeparatedFromPrevious());

                        if (ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                        {
                            method.AddForEachStatement("scheduleEntry", "_messagesToSchedule", loop =>
                            {
                                loop.SeparatedFromPrevious();
                                loop.AddStatement(
                                    "await messageScheduler.SchedulePublish(scheduleEntry.Scheduled, scheduleEntry.Message, cancellationToken).ConfigureAwait(false);");
                            });
                        }
                    });

                    if (!ExecutionContext.Settings.GetEventingSettings().EnableScheduledPublishing())
                    {
                        return;
                    }

                    @class.AddNestedClass("ScheduleEntry", nested =>
                    {
                        nested.Private();
                        nested.AddConstructor(ctor =>
                        {
                            ctor.Private();
                            ctor.AddParameter("object", "message", param => param.IntroduceProperty(prop => prop.ReadOnly()));
                            ctor.AddParameter("DateTime", "scheduled", param => param.IntroduceProperty(prop => prop.ReadOnly()));
                        });
                        nested.AddMethod("ScheduleEntry", "ForScheduled", method =>
                        {
                            method.Static();
                            method.AddParameter("object", "message");
                            method.AddParameter("DateTime", "scheduled");
                            method.AddStatement("return new ScheduleEntry(message, scheduled);");
                        });
                        nested.AddMethod("ScheduleEntry", "ForDelay", method =>
                        {
                            method.Static();
                            method.AddParameter("object", "message");
                            method.AddParameter("TimeSpan", "delay");
                            method.AddStatement("return new ScheduleEntry(message, DateTime.UtcNow.Add(delay));");
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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