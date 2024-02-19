using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.RequestResponseMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RequestResponseMessageTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponseMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RequestResponseMessageTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("RequestResponseMessage", @class =>
                {
                    @class.AddProperty(@class.Name, "Instance", prop => prop.Static().WithInitialValue($"new {@class.Name}()").WithoutSetter());
                })
                .AddClass("RequestResponseMessage", @class =>
                {
                    @class.AddGenericParameter("T", out var typeT);
                    @class.AddConstructor(ctor => ctor.AddStatement("Payload = default!;"));
                    @class.AddConstructor(ctor => ctor.AddParameter(typeT, "payload", param => param.IntroduceProperty()));
                });
        }

        public override bool CanRunTemplate()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);
            var relevantCommands = services.GetElementsOfType("Command")
                .Where(p => p.HasStereotype("MassTransit Consumer"));
            var relevantQueries = services.GetElementsOfType("Query")
                .Where(p => p.HasStereotype("MassTransit Consumer"));
            return relevantCommands.Concat(relevantQueries).Any();
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