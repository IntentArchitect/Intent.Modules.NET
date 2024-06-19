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
                    @class.AddField("List<MessageConfiguration>", "_messageTypes", p => p.PrivateReadOnly());
                    @class.AddField("Dictionary<Type, MessageConfiguration>", "_typeLookup", p => p.PrivateReadOnly());
                    @class.AddField("string", "_environmentPrefix", p => p.PrivateReadOnly().WithAssignment("\"\""));
                    @class.AddField("string", "_applicationPrefix", p => p.PrivateReadOnly().WithAssignment("\"\""));
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
            var applicationPrefix = configuration[$""Solace:Application""] ?? """";
			if (applicationPrefix != """")
			{
				if (!applicationPrefix.EndsWith(""/""))
					applicationPrefix += ""/"";
				_applicationPrefix = applicationPrefix;
			}

			_messageTypes = new List< MessageConfiguration>();
			LoadMessageTypes();
			_typeLookup = new Dictionary<Type, MessageConfiguration>();
			foreach (var message in _messageTypes)
			{
				_typeLookup.Add(message.MessageType, message);
			}
".ConvertToStatements());
                    });

                    @class.AddProperty($"IReadOnlyList<{this.GetMessageConfigurationName()}>", "MessageTypes", p => p.ReadOnly().Getter.WithExpressionImplementation("_messageTypes"));

                    @class.AddMethod("void", "LoadMessageTypes", method =>
                    {
                        method.Private();

                        if (_subscribedMessageModels.Any() || _publishedMessageModels.Any())
                        {
                            method.AddStatement("//Integration Events (Messages)");
                        }
                        foreach (var message in _subscribedMessageModels)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(message);
                            var subscriptionMechaism = "SubscriptionType.ViaTopic";
                            if (!HasRoutingCustomizations(message.InternalElement))
                            {
                                method.AddStatement($"_messageTypes.Add(GetDefaultMessageConfig<{messageTypeName}>({subscriptionMechaism}));");
                            }
                            else
                            {
                                var invocation = new CSharpInvocationStatement($"MessageConfiguration.Create<{messageTypeName}>");
                                GetRoutingCustomizations(message.InternalElement, out var pubDest, out var subDest);
                                if (pubDest != null)
                                {
                                    invocation.AddArgument($"GetDestination<{messageTypeName}>(\"{pubDest}\")");
                                }
                                else
                                {
                                    invocation.AddArgument($"GetDefaultDestination<{messageTypeName}>()");
                                }
                                if (subDest != null)
                                {
                                    invocation.AddArgument($"GetDestination<{messageTypeName}>(\"{subDest}\", {subscriptionMechaism})");
                                }
                                else
                                {
                                    invocation.AddArgument($"GetDefaultDestination<{messageTypeName}>(), {subscriptionMechaism}");
                                }
                                invocation.WithArgumentsOnNewLines().WithoutSemicolon();
                                method.AddStatement(new CSharpInvocationStatement("_messageTypes.Add").AddArgument(invocation));
                            }
                        }
                        foreach (var message in _publishedMessageModels)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(message);
                            if (!HasRoutingCustomizations(message.InternalElement))
                            {
                                method.AddStatement($"_messageTypes.Add(GetDefaultMessageConfig<{messageTypeName}>());");
                            }
                            else
                            {
                                var invocation = new CSharpInvocationStatement($"MessageConfiguration.Create<{messageTypeName}>");
                                GetRoutingCustomizations(message.InternalElement, out var pubDest, out var subDest);
                                if (pubDest != null)
                                {
                                    invocation.AddArgument($"GetDestination<{messageTypeName}>(\"{pubDest}\")");
                                }
                                else
                                {
                                    invocation.AddArgument($"GetDefaultDestination<{messageTypeName}>()");
                                }
                                invocation.WithArgumentsOnNewLines().WithoutSemicolon();
                                method.AddStatement(new CSharpInvocationStatement("_messageTypes.Add").AddArgument(invocation));
                            }
                        }
                        if (_subscribedIntegrationCommandModels.Any() || _publishedIntegrationCommandModels.Any())
                        {
                            method.AddStatement("//Integration Commands");
                        }
                        foreach (var command in _subscribedIntegrationCommandModels)
                        {
                            var commandTypeName = this.GetIntegrationCommandName(command);
                            var subscriptionMechaism = "SubscriptionType.ViaQueue";

                            if (!HasRoutingCustomizations(command.InternalElement))
                            {
                                method.AddStatement($"_messageTypes.Add(GetDefaultMessageConfig<{commandTypeName}>({subscriptionMechaism}));");
                            }
                            else
                            {
                                var invocation = new CSharpInvocationStatement($"MessageConfiguration.Create<{commandTypeName}>");
                                GetRoutingCustomizations(command.InternalElement, out var pubDest, out var subDest);
                                if (pubDest != null)
                                {
                                    invocation.AddArgument($"GetDestination<{commandTypeName}>(\"{pubDest}\")");
                                }
                                else
                                {
                                    invocation.AddArgument($"GetDefaultDestination<{commandTypeName}>()");
                                }
                                if (subDest != null)
                                {
                                    invocation.AddArgument($"GetDestination<{commandTypeName}>(\"{subDest}\", {subscriptionMechaism})");
                                }
                                else
                                {
                                    invocation.AddArgument($"GetDefaultDestination<{commandTypeName}>(), {subscriptionMechaism}");
                                }
                                invocation.WithArgumentsOnNewLines().WithoutSemicolon();
                                method.AddStatement(new CSharpInvocationStatement("_messageTypes.Add").AddArgument(invocation));
                            }

                        }
                        foreach (var command in _publishedIntegrationCommandModels)
                        {
                            var commandTypeName = this.GetIntegrationCommandName(command);
                            if (!HasRoutingCustomizations(command.InternalElement))
                            {
                                method.AddStatement($"_messageTypes.Add(GetDefaultMessageConfig<{commandTypeName}>());");
                            }
                            else
                            {
                                var invocation = new CSharpInvocationStatement($"MessageConfiguration.Create<{commandTypeName}>");
                                GetRoutingCustomizations(command.InternalElement, out var pubDest, out var subDest);
                                if (pubDest != null)
                                {
                                    invocation.AddArgument($"GetDestination<{commandTypeName}>(\"{pubDest}\")");
                                }
                                else
                                {
                                    invocation.AddArgument($"GetDefaultDestination<{commandTypeName}>()");
                                }
                                invocation.WithArgumentsOnNewLines().WithoutSemicolon();
                                method.AddStatement(new CSharpInvocationStatement("_messageTypes.Add").AddArgument(invocation));
                            }

                        }
                    });

                    @class.AddMethod($"{this.GetMessageConfigurationName()}", "GetConfig", method =>
                    {
                        method.AddParameter("Type", "messageType");
                        method.AddStatement("return _typeLookup[messageType];");
                    });

                    @class.AddMethod($"{this.GetMessageConfigurationName()}", "GetConfig", method =>
                    {
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddStatement($"return GetConfig(typeof({tMessage}));");
                    });

                    @class.AddMethod($"{this.GetMessageConfigurationName()}", "GetDefaultMessageConfig", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage", out var tMessage);
                        method.AddParameter("SubscriptionType", "subscriptionType", p => p.WithDefaultValue("SubscriptionType.None"));
                        method.AddStatements(@"switch (subscriptionType)
            {
                case SubscriptionType.ViaTopic:
                    return MessageConfiguration.Create<TMessage>(GetDefaultDestination<TMessage>(),
							GetDefaultDestination<TMessage>(SubscriptionType.ViaTopic));
                case SubscriptionType.ViaQueue:
                    return MessageConfiguration.Create<TMessage>(GetDefaultDestination<TMessage>(),
							GetDefaultDestination<TMessage>(SubscriptionType.ViaQueue));
                case SubscriptionType.None:
                default:
                    return MessageConfiguration.Create<TMessage>(GetDefaultDestination<TMessage>());
            }".ConvertToStatements());
                    });

                    @class.AddMethod($"string", "GetDestination", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage", out var tMessage)
                            .AddParameter("string", "logicalPath")
                            .AddParameter("SubscriptionType", "subscriptionType", p => p.WithDefaultValue("SubscriptionType.None"));
                        method.AddStatements(@"var messageType = typeof(TMessage);
            switch (subscriptionType)
            {
                case SubscriptionType.ViaTopic:
                    return $""{_environmentPrefix}{_applicationPrefix}{logicalPath}"";
                case SubscriptionType.ViaQueue:
                case SubscriptionType.None:
                default:
                    return $""{_environmentPrefix}{logicalPath}"";
            }".ConvertToStatements());
                    });

                    @class.AddMethod($"string", "GetDefaultDestination", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TMessage", out var tMessage)
                            .AddParameter("SubscriptionType", "subscriptionType", p => p.WithDefaultValue("SubscriptionType.None"));
                        method.AddStatements(@"var messageType = typeof(TMessage);
			string namepacePrefix = messageType.Namespace == null ? """" : $""{messageType.Namespace.Replace('.', '/')}/"";
			return GetDestination<TMessage>($""{namepacePrefix}{messageType.Name}"", subscriptionType);".ConvertToStatements());
                    });

                })
                .AddEnum("SubscriptionType", e =>
                {
                    e.AddLiteral("None");
                    e.AddLiteral("ViaTopic");
                    e.AddLiteral("ViaQueue");
                });
        }

        private bool HasRoutingCustomizations(IElement message)
        {
            if (message.HasStereotype(Api.Constants.StereotypeIds.Publishing))
            {
                return true;
            }
            if (message.AssociatedElements.Any(e => e.Association.TargetEnd.HasStereotype(Api.Constants.StereotypeIds.Subscribing) == true))
            {
                return true;
            }
            return false;
        }

        private void GetRoutingCustomizations(IElement message, out string? publishingPath, out string? subscribingPath)
        {
            publishingPath = null;
            subscribingPath = null;
            if (message.HasStereotype(Api.Constants.StereotypeIds.Publishing))
            {
                publishingPath = message.GetStereotypeProperty<string>(Api.Constants.StereotypeIds.Publishing, "Destination");
            }
            if (message.AssociatedElements.Any(e => e.Association.TargetEnd.HasStereotype(Api.Constants.StereotypeIds.Subscribing) == true))
            {
                subscribingPath = message.AssociatedElements.First(e => e.Association.TargetEnd.HasStereotype(Api.Constants.StereotypeIds.Subscribing) == true).Association.TargetEnd.GetStereotypeProperty<string>(Api.Constants.StereotypeIds.Subscribing, "Queue");
            }
        }


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
                    //If I've already covered the message in in the Sub don't do it here
                    .Except(_subscribedMessageModels)
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
                    //If I've already covered the message in in the Sub don't do it here
                    .Except(_subscribedIntegrationCommandModels)
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