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

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageTableAdapterInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageTableAdapterInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageTableAdapterInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageTableAdapterInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureDataTables(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddUsing("System")
            .AddInterface($"ITableAdapter", @interface =>
            {
                @interface
                    .Internal()
                    .AddGenericParameter("TDomain", out var tDomain)
                    .AddGenericParameter("TTable", out var tDocument, g => g.Covariant())
                    .AddGenericTypeConstraint(tDomain, c => c
                        .AddType("class"))
                    .AddGenericTypeConstraint(tDocument, c => c
                        .AddType($"{this.GetTableStorageTableAdapterInterfaceName()}<{tDomain}, {tDocument}>"));

                @interface.ImplementsInterfaces(UseType("ITableAdapter"));

                @interface.AddMethod(tDocument, "PopulateFromEntity", c => c
                    .AddParameter(tDomain, "entity"));

                @interface.AddMethod(tDomain, "ToEntity", c => c
                    .AddParameter($"{tDomain}?", "entity", p => p.WithDefaultValue("null")));
            })
            .AddInterface($"ITableAdapter", @interface =>
            {
                @interface.Internal();
                @interface.ImplementsInterfaces(UseType("Azure.Data.Tables.ITableEntity"));
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