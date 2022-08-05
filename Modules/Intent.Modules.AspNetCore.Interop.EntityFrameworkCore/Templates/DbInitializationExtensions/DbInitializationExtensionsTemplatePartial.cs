using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Interop.EntityFrameworkCore.Templates.DbInitializationExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DbInitializationExtensionsTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AspNetCore.Interop.EntityFrameworkCore.DbInitializationExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbInitializationExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }
        
        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("EnsureDbCreation")
                .HasDependency(this)
                .WithPriority(-50));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DbInitializationExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}