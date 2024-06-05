using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates.MessageConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.MessageConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"MessageConfiguration", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
						ctor.AddParameter("Type", "messageType", param =>
                        {
                            param.IntroduceProperty();
                        });
						ctor.AddParameter("string", "publishedDestination", param =>
						{
							param.IntroduceProperty();
						});
						ctor.AddParameter("SubscriptionType", "subscriptionType", param =>
						{
							param.IntroduceProperty().WithDefaultValue("SubscriptionType.None");
						});
						ctor.AddParameter("string?", "subscribeDestination", param =>
						{
							param.IntroduceProperty().WithDefaultValue("null");
						});
						ctor.AddStatement("Name = GetName(messageType);");
                    });
					@class.AddMethod("MessageConfiguration", "Create", method =>
					{
                        method.Static();
						method.AddGenericParameter("TMessage");
						method
                            .AddParameter("string", "publishedDestination")
                            .AddParameter("SubscriptionType", "subscriptionType", p => p.WithDefaultValue("SubscriptionType.None"))
                            .AddParameter("string?", "subscribeDestination", p => p.WithDefaultValue("null"));
						method.AddStatement("return new MessageConfiguration(typeof(TMessage), publishedDestination, subscriptionType, subscribeDestination);");
					});

					@class.AddProperty("string", "Name");
					@class.AddMethod("string", "GetName", method => 
                    {
                        method.Private();
                        method.AddParameter("Type", "type");
                        method.AddStatement("return $\"{type.Namespace}.{type.Name}\";");
                    });
                })
                .AddEnum("SubscriptionType", e => 
                {
                    e.AddLiteral("None");
					e.AddLiteral("ViaTopic");
					e.AddLiteral("ViaQueue");
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
    }
}