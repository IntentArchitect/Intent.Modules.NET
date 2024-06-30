using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates.MessageRegistry
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageRegistryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.MessageRegistry";

        private IReadOnlyCollection<MessageModel> _subscribedMessageModels;
        private IReadOnlyCollection<MessageModel> _publishedMessageModels;
        private IReadOnlyCollection<IntegrationCommandModel> _subscribedIntegrationCommandModels;
        private IReadOnlyCollection<IntegrationCommandModel> _publishedIntegrationCommandModels;


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageRegistryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            LoadEventsAndCommands(outputTarget);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"MessageRegistry", @class =>
                {
                    @class.AddField("Dictionary<Type, string>", "_typeNameMap", p => p.PrivateReadOnly().WithAssignment("new Dictionary<Type, string>()"));
                    @class.AddField("Dictionary<Type, PublishingInfo>", "_publishedMessages", p => p.PrivateReadOnly().WithAssignment("new Dictionary<Type, PublishingInfo>()"));
                    @class.AddField("List<QueueConfig>", "_queues", p => p.PrivateReadOnly().WithAssignment("new List<QueueConfig>()"));
                    @class.AddField("Dictionary<string, string>", "_replacementVariables", p => p.PrivateReadOnly().WithAssignment("new Dictionary<string, string>()"));
                    @class.AddField("string", "_environmentPrefix", p => p.PrivateReadOnly().WithAssignment("\"\""));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration");
                        ctor.AddStatements(@"var environmentPrefix = configuration[$""Solace:EnvironmentPrefix""] ?? """";
			if (environmentPrefix != """")
			{
				if (!environmentPrefix.EndsWith(""/""))
					environmentPrefix += ""/"";
				_environmentPrefix = environmentPrefix;
			}
            LoadReplacementVariables(configuration);
            RegisterMessageTypes();
".ConvertToStatements());
                    });

                    @class.AddProperty($"IReadOnlyDictionary<Type, string>", "MessageTypes", p => p.ReadOnly().Getter.WithExpressionImplementation("_typeNameMap"));
                    @class.AddProperty($"IReadOnlyDictionary<Type, PublishingInfo>", "PublishedMessages", p => p.ReadOnly().Getter.WithExpressionImplementation("_publishedMessages"));
                    @class.AddProperty($"IReadOnlyList<QueueConfig>", "Queues", p => p.ReadOnly().Getter.WithExpressionImplementation("_queues"));

                    @class.AddMethod("void", "RegisterMessageTypes", method =>
                    {
                        method.Private();

                        if (_subscribedMessageModels.Any() || _publishedMessageModels.Any())
                        {
                            method.AddStatement("//Integration Events (Messages)");
                        }
                        var eventQueues = GetQueues(_subscribedMessageModels.Select(x => x.InternalElement), m => this.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", m.Id), true);
                        foreach (var queue in eventQueues)
                        {
                            var invocation = new CSharpInvocationStatement($"RegisterQueue");
                            invocation.AddArgument($"\"{queue.Name}\"");

                            invocation.AddArgument(new CSharpLambdaBlock("queue"), a =>
                            {
                                var stmt = (CSharpLambdaBlock)a;
                                foreach (var message in queue.Messages)
                                {
                                    var subInvocation = new CSharpInvocationStatement($"SubscribeViaTopic<{this.GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", message.Message.Id)}>");
                                    subInvocation.AddArgument("queue");
                                    if (message.SubscribedTopic != null)
                                    {
                                        subInvocation.AddArgument($"topicName: \"{message.SubscribedTopic}\"");

                                    }
                                    stmt.AddStatement(subInvocation);
                                }
                            });

                            if (queue.MaxFlows != null)
                            {
                                invocation.AddArgument($"maxFlows: {queue.MaxFlows}");
                            }

                            if (queue.Selector != null)
                            {
                                invocation.AddArgument($"selector: \"{queue.Selector}\"");
                            }
                            method.AddStatement(invocation);
                        }

                        foreach (var message in _publishedMessageModels)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(message);
                            GetPublishingCustomizations(message.InternalElement, out var topicName, out var priority);

                            var invocation = new CSharpInvocationStatement($"PublishToTopic<{messageTypeName}>");
                            if (topicName != null)
                            {
                                invocation.AddArgument($"topicName: \"{topicName}\"");
                            }
                            if (priority != null)
                            {
                                invocation.AddArgument($"priority: {priority}");
                            }
                            method.AddStatement(invocation);
                        }
                        if (_subscribedIntegrationCommandModels.Any() || _publishedIntegrationCommandModels.Any())
                        {
                            method.AddStatement("//Integration Commands");
                        }
                        var commandQueues = GetQueues(_subscribedIntegrationCommandModels.Select(x => x.InternalElement), m => this.GetTypeName("Intent.Eventing.Contracts.IntegrationCommand", m.Id), false);
                        foreach (var queue in commandQueues)
                        {
                            var invocation = new CSharpInvocationStatement($"RegisterQueue");
                            invocation.AddArgument($"\"{queue.Name}\"");

                            invocation.AddArgument(new CSharpLambdaBlock("queue"), a =>
                            {
                                var stmt = (CSharpLambdaBlock)a;
                                foreach (var message in queue.Messages)
                                {
                                    var subInvocation = new CSharpInvocationStatement($"SubscribeViaQueue<{this.GetTypeName("Intent.Eventing.Contracts.IntegrationCommand", message.Message.Id)}>");
                                    subInvocation.AddArgument("queue");
                                    stmt.AddStatement(subInvocation);
                                }
                            });

                            if (queue.MaxFlows != null)
                            {
                                invocation.AddArgument($"maxFlows: {queue.MaxFlows}");
                            }

                            if (queue.Selector != null)
                            {
                                invocation.AddArgument($"selector: \"{queue.Selector}\"");
                            }
                            method.AddStatement(invocation);
                        }

                        foreach (var command in _publishedIntegrationCommandModels)
                        {
                            var commandTypeName = this.GetIntegrationCommandName(command);
                            GetPublishingCustomizations(command.InternalElement, out var queueName, out var priority);

                            var invocation = new CSharpInvocationStatement($"PublishToQueue<{commandTypeName}>");
                            if (queueName != null)
                            {
                                invocation.AddArgument($"queueName: \"{queueName}\"");
                            }
                            if (priority != null)
                            {
                                invocation.AddArgument($"priority: {priority}");
                            }
                            method.AddStatement(invocation);
                        }
                    });

                    @class.AddMethod($"void", "RegisterQueue", method =>
                    {
                        method
                            .Private()
                            .AddParameter("string", "logicalQueueName")
                            .AddParameter("Action<QueueConfig>", "registerSubscribers")
                            .AddParameter("int?", "maxFlows", p => p.WithDefaultValue("null"))
                            .AddParameter("string?", "selector", p => p.WithDefaultValue("null"))
                            ;
                        method.AddStatements(@"if (selector != null)
            {
                selector = ResolveSelector(selector);
            }
            var queue = new QueueConfig(ResolveLocation(logicalQueueName), maxFlows, selector, new List<SubscribesToQueueInfo>());
            registerSubscribers(queue);
            _queues.Add(queue);".ConvertToStatements());
                    });

                    @class.AddMethod($"void", "PublishToTopic", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            .AddParameter("string?", "topicName", p => p.WithDefaultValue("null"))
                            .AddParameter("int?", "priority", p => p.WithDefaultValue("null"))
                            ;
                        method.AddStatement("PublishInternal<TMessage>(topicName ?? GetDefaultMessageDestination<TMessage>(), priority);");
                    });

                    @class.AddMethod($"void", "PublishToQueue", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            .AddParameter("string?", "queueName", p => p.WithDefaultValue("null"))
                            .AddParameter("int?", "priority", p => p.WithDefaultValue("null"))
                            ;
                        method.AddStatement("PublishInternal<TMessage>(queueName ?? GetDefaultMessageDestination<TMessage>(), priority);");
                    });

                    @class.AddMethod($"void", "PublishInternal", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            .AddParameter("string", "destination")
                            .AddParameter("int?", "priority", p => p.WithDefaultValue("null"))
                            ;
                        method.AddStatement("AddMessageType<TMessage>();");
                        method.AddStatement("_publishedMessages.Add(typeof(TMessage), new PublishingInfo(typeof(TMessage), ResolveLocation(destination), priority));");
                    });

                    @class.AddMethod($"void", "SubscribeViaTopic", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            .AddParameter("QueueConfig", "queue")
                            .AddParameter("string?", "topicName", p => p.WithDefaultValue("null"))
                            ;
                        method.AddStatement("SubscribeInternal<TMessage>(queue, topicName ?? GetDefaultMessageDestination<TMessage>());");
                    });

                    @class.AddMethod($"void", "SubscribeViaQueue", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            .AddParameter("QueueConfig", "queue")
                            ;
                        method.AddStatement("SubscribeInternal<TMessage>(queue);");
                    });

                    @class.AddMethod($"void", "SubscribeInternal", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            .AddParameter("QueueConfig", "queue")
                            .AddParameter("string?", "topicName", p => p.WithDefaultValue("null"))
                            ;
                        method.AddStatement("AddMessageType<TMessage>();");
                        method.AddStatement(@"queue.SubscribedMessages.Add(new SubscribesToQueueInfo(
                    typeof(TMessage),
                    topicName != null ? ResolveLocation(topicName) : null
                    ));");
                    });

                    @class.AddMethod($"void", "AddMessageType", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            ;
                        method.AddStatements(@"if (!_typeNameMap.ContainsKey(typeof(TMessage)))
            {
                string messageTypeName = GetMessageTypeName<TMessage>();
                _typeNameMap.Add(typeof(TMessage), messageTypeName);
            }".ConvertToStatements());
                    });

                    @class.AddMethod($"string", "GetDefaultMessageDestination", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            ;
                        method.AddStatement("var messageType = typeof(TMessage);");
                        method.AddStatement("string namepacePrefix = messageType.Namespace == null ? \"\" : $\"{messageType.Namespace.Replace('.', '/')}/\";");
                        method.AddStatement("return $\"{namepacePrefix}{messageType.Name}\";");
                    });

                    @class.AddMethod($"string", "GetMessageTypeName", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage")
                            ;
                        method.AddStatement("var type = typeof(TMessage);");
                        method.AddStatement("return $\"{type.Namespace}.{type.Name}\";");
                    });

                    this.UseType(this.GetSolaceConfigurationName());

                    @class.AddMethod($"void", "LoadReplacementVariables", method =>
                    {
                        method
                            .Private()
                            .AddParameter("IConfiguration", "configuration")
                            ;
                        method.AddStatements(@"var config = configuration.GetSection(""Solace"").Get<SolaceConfiguration.SolaceConfig>();
            if (config == null) throw new Exception(""No Solace configuration found in appsettings.json"");

            var properties = config.GetType().GetProperties();
            foreach (var property in properties)
            {
                _replacementVariables.Add(property.Name, property.GetValue(config)?.ToString() ?? """");
            }".ConvertToStatements());
                    });

                    @class.AddMethod($"string", "ReplaceVariables", method =>
                    {
                        method
                            .Private()
                            .AddParameter("string", "value")
                            ;
                        method.AddStatements(@"if (value.Contains(""{""))
            {
                foreach (var property in _replacementVariables)
                {
                    value = value.Replace($""{{{property.Key}}}"", property.Value);
                }
            }
            return value;".ConvertToStatements());
                    });

                    @class.AddMethod($"string", "ResolveSelector", method =>
                    {
                        method
                            .Private()
                            .AddParameter("string", "logicalSelector")
                            ;
                        method.AddStatement("return ReplaceVariables(logicalSelector);");
                    });

                    @class.AddMethod($"string", "ResolveLocation", method =>
                    {
                        method
                            .Private()
                            .AddParameter("string", "logicalLocation")
                            ;
                        method.AddStatement("return $\"{_environmentPrefix}{ReplaceVariables(logicalLocation)}\";");
                    });
                }).AddRecord("PublishingInfo", record =>
                {
                    record.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("Type", "PublishedMessage");
                        ctor.AddParameter("string", "PublishTo");
                        ctor.AddParameter("int?", "Priority");
                    });
                }).AddRecord("SubscribesToQueueInfo", record =>
                {
                    record.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("Type", "MessageType");
                        ctor.AddParameter("string?", "TopicName");
                    });
                }).AddRecord("QueueConfig", record =>
                {
                    record.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "QueueName");
                        ctor.AddParameter("int?", "MaxFlows");
                        ctor.AddParameter("string?", "Selector");
                        ctor.AddParameter("List<SubscribesToQueueInfo>", "SubscribedMessages");
                    });
                });
        }

        private void GetPublishingCustomizations(IElement message, out string? destination, out int? priority)
        {
            destination = null;
            priority = null;
            if (message.HasStereotype(Api.Constants.StereotypeIds.Publishing))
            {
                destination = message.GetStereotypeProperty<string>(Api.Constants.StereotypeIds.Publishing, "Destination");
                priority = message.GetStereotypeProperty<int?>(Api.Constants.StereotypeIds.Publishing, "Priority");
            }
        }

        private List<SubscriptionQueue> GetQueues(IEnumerable<IElement> models, Func<IElement, string> getTypeName, bool topicSubscription)
        {
            var result = new Dictionary<string, SubscriptionQueue>();
            foreach (var message in models)
            {
                string queueName;
                int? maxFlow = null;
                string? selector = null;
                string? subscriptioTopic = null;
                if (message.AssociatedElements.Any(e => e.Association.TargetEnd.HasStereotype(Api.Constants.StereotypeIds.Subscribing) == true))
                {
                    var subscription = message.AssociatedElements.First(e => e.Association.TargetEnd.HasStereotype(Api.Constants.StereotypeIds.Subscribing) == true).Association.TargetEnd.GetStereotype(Api.Constants.StereotypeIds.Subscribing);
                    var queue = subscription.GetProperty<IElement>("Queue");
                    queueName = queue.Name;
                    selector = queue.GetStereotypeProperty<string?>("Queue Config", "Selector");
                    maxFlow = queue.GetStereotypeProperty<int?>("Queue Config", "Max Flows");
                    subscriptioTopic = subscription.GetProperty<string?>("Topic") == null ? null : $"\"{subscription.GetProperty<string?>("Topic")}\"";
                    if (string.IsNullOrEmpty(subscriptioTopic))
                    {
                        if (message.HasStereotype(Api.Constants.StereotypeIds.Publishing))
                        {
                            subscriptioTopic = message.GetStereotypeProperty<string>(Api.Constants.StereotypeIds.Publishing, "Destination");
                        }
                        else
                        {
                            subscriptioTopic = getTypeName(message).Replace(".", "//");
                        }
                    }
                }
                else
                {
                    if (message.HasStereotype(Api.Constants.StereotypeIds.Publishing))
                    {
                        queueName = message.GetStereotypeProperty<string>(Api.Constants.StereotypeIds.Publishing, "Destination");
                    }
                    else
                    {
                        queueName = getTypeName(message).Replace(".", "//");
                    }

                    if (topicSubscription)
                    {
                        queueName = $"{{Application}}/{queueName}";
                    }
                }
                if (!result.TryGetValue(queueName, out var q))
                {
                    q = new SubscriptionQueue(queueName, maxFlow, selector, new List<SubscriptionMessage> { new SubscriptionMessage(message, subscriptioTopic) });
                    result.Add(queueName, q);
                }
                else
                {
                    q.Messages.Add(new SubscriptionMessage(message, subscriptioTopic));
                }
            }
            return result.Values.ToList();
        }

        internal record SubscriptionQueue(string Name, int? MaxFlows, string? Selector, List<SubscriptionMessage> Messages);
        internal record SubscriptionMessage(IElement Message, string? SubscribedTopic);

        private void LoadEventsAndCommands(IOutputTarget outputTarget)
        {
            //Events
            var serviceDesignerSubEvents = ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationEventSubscriptions())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            var eventingDesignerSubEvents = ExecutionContext.MetadataManager
                .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                .SelectMany(x => x.SubscribedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            _subscribedMessageModels = Enumerable.Empty<MessageModel>()
                .Concat(serviceDesignerSubEvents)
                .Concat(eventingDesignerSubEvents)
                .OrderBy(x => x.Name)
                .ToArray();

            var serviceDesignerPubEvents = ExecutionContext.MetadataManager
                .GetExplicitlyPublishedMessageModels(OutputTarget.Application);

            var eventingDesignerPubEvents = ExecutionContext.MetadataManager
                .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                .SelectMany(x => x.PublishedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            _publishedMessageModels = Enumerable.Empty<MessageModel>()
                    .Concat(serviceDesignerPubEvents)
                    .Concat(eventingDesignerPubEvents)
                    .OrderBy(x => x.Name)
                    .ToArray();

            //Commands
            var serviceDesignerSubCommands = ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationCommandSubscriptions())
                .Select(x => x.TypeReference.Element.AsIntegrationCommandModel());

            _subscribedIntegrationCommandModels = Enumerable.Empty<IntegrationCommandModel>()
                .Concat(serviceDesignerSubCommands)
                .OrderBy(x => x.Name)
                .ToArray();

            var serviceDesignerPubCommands = ExecutionContext.MetadataManager
                .GetExplicitlySentIntegrationCommandDispatches(OutputTarget.Application.Id)
                .Select(x => x.TypeReference.Element.AsIntegrationCommandModel());


            _publishedIntegrationCommandModels = Enumerable.Empty<IntegrationCommandModel>()
                    .Concat(serviceDesignerPubCommands)
                    .OrderBy(x => x.Name)
                    .ToArray();
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