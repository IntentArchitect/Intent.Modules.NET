using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbInitializationExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DbInitializationExtensionsTemplate : CSharpTemplateBase<object>
    {
        private ICSharpFileBuilderTemplate _startupTemplate;
        public const string TemplateId = "Intent.EntityFrameworkCore.DbInitializationExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbInitializationExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            _startupTemplate.CSharpFile.AddUsing(Namespace);
            _startupTemplate.CSharpFile.AfterBuild(file =>
            {
                var method = file.Classes.First().FindMethod("Configure");
                method.AddStatement(@"
            if (Configuration.GetValue<bool>(""Cosmos:EnsureDbCreated""))
            {
                app.EnsureDbCreationAsync().GetAwaiter().GetResult();
            }", s => s.SeparatedFromPrevious());
            });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos() &&
                   TryGetTemplate(TemplateRoles.Distribution.WebApi.Startup, out _startupTemplate);
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