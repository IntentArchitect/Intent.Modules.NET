using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.IntegrationTesting;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.Templates.PopulateIdsSpecimenBuilder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PopulateIdsSpecimenBuilderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTests.CRUD.PopulateIdsSpecimenBuilder";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PopulateIdsSpecimenBuilderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AutoFixture(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("AutoFixture.Kernel")
                .AddUsing("System.Reflection")
                .AddClass($"PopulateIdsSpecimenBuilder", @class =>
                {
                    @class.ImplementsInterface("ISpecimenBuilder");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("Dictionary<string, object>", "idsToReplace", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("object", "Create", method =>
                    {
                        method
                            .AddParameter("object", "request")
                            .AddParameter("ISpecimenContext", "context")
                            .AddStatement("var propertyInfo = request as PropertyInfo;")
                            .AddIfStatement("propertyInfo != null", stmt =>
                            {
                                stmt.AddIfStatement("_idsToReplace.TryGetValue(propertyInfo.Name, out object? value)", ifs => ifs.AddStatement("return value;"));
                                stmt.AddIfStatement("propertyInfo.Name.EndsWith(\"Id\")", ifs => ifs.AddStatement("return new OmitSpecimen();"));
                            })
                            .AddStatement("return new NoSpecimen();")
                            ;
                    });
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