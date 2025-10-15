using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs.Templates.SqsSubscriptionOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class SqsSubscriptionOptionsTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Aws.Sqs.SqsSubscriptionOptions";
        public const string SubscriptionEntry = "SubscriptionEntry";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SqsSubscriptionOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"SqsSubscriptionOptions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}
