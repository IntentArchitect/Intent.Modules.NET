using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.UtcDateTimeOffsetConverter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UtcDateTimeOffsetConverterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.UtcDateTimeOffsetConverter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UtcDateTimeOffsetConverterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.EntityFrameworkCore.Storage.ValueConversion")
                .AddClass($"UtcDateTimeOffsetConverter", @class =>
                {
                    @class.WithComments(new[]
                    {
                        "/// <summary>",
                        "/// Postgres requires DateTimeOffset to be stored as Utc , this converter ensures that is the case",
                        "/// </summary>"
                    });
                    @class.WithBaseType("ValueConverter<DateTimeOffset, DateTimeOffset>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.CallsBase(baseCall =>
                        {
                            baseCall
                                .AddArgument("nonUtc => nonUtc.ToUniversalTime()")
                                .AddArgument("utc => utc");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql();
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