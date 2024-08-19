using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.CollectionWrapper
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class CollectionWrapperTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Entities.CollectionWrapper";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CollectionWrapperTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"CollectionWrapper",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                   !ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters();
        }
    }
}