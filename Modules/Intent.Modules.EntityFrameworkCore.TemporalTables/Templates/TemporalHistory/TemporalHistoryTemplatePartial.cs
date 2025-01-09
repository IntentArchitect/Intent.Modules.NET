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

namespace Intent.Modules.EntityFrameworkCore.TemporalTables.Templates.TemporalHistory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TemporalHistoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.TemporalTables.TemporalHistory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TemporalHistoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"TemporalHistory", @class =>
                {
                    @class.AddGenericParameter("TEntity");

                    @class.AddProperty("TEntity", "Entity", prop => prop.PrivateSetter());
                    @class.AddProperty(UseType("System.DateTime"), "ValidFrom", prop => prop.PrivateSetter());
                    @class.AddProperty(UseType("System.DateTime"), "ValidTo", prop => prop.PrivateSetter());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("TEntity", "entity");
                        ctor.AddParameter("DateTime", "validFrom");
                        ctor.AddParameter("DateTime", "validTo");

                        ctor.AddObjectInitStatement("Entity", "entity;");
                        ctor.AddObjectInitStatement("ValidFrom", "validFrom;");
                        ctor.AddObjectInitStatement("ValidTo", "validTo;");
                    });
                })
                .AddRecord("TemporalHistoryQueryOptions", @record =>
                {
                    @record.AddPrimaryConstructor(ctor =>
                    {
                        ctor.AddParameter("TemporalHistoryQueryType?", "QueryType", cfg => cfg.WithDefaultValue("TemporalHistoryQueryType.All"))
                            .AddParameter("DateTime?", "DateFrom", cfg => cfg.WithDefaultValue("null"))
                            .AddParameter("DateTime?", "DateTo", cfg => cfg.WithDefaultValue("null"));
                    });
                })
                .AddEnum("TemporalHistoryQueryType", @enum =>
                {
                    @enum.AddLiteral("All");
                    @enum.AddLiteral("AsOf");
                    @enum.AddLiteral("Between");
                    @enum.AddLiteral("ContainedIn");
                    @enum.AddLiteral("FromTo");
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