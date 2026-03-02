using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Assertions.AssertionClass;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class AssertionClassTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public AssertionClassTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
    {
        AddTypeSource(CommandModelsTemplate.TemplateId);
        AddTypeSource(TemplateRoles.Domain.DataContract);
        AddTypeSource(TemplateRoles.Domain.ValueObject);
        AddTypeSource(CSharpTypeSource.Create(ExecutionContext, TemplateRoles.Application.Contracts.Dto, "IEnumerable<{0}>"));
        AddTypeSource(CSharpTypeSource.Create(ExecutionContext, TemplateRoles.Domain.Entity.Primary, "IEnumerable<{0}>"));

        CSharpFile = new CSharpFile(this.GetNamespace(model.Name.Pluralize()), this.GetFolderPath(model.Name.Pluralize()))
            .AddClass($"{model.Name.ToPascalCase()}Assertions")
            .OnBuild(file =>
            {
                file.AddUsing("FluentAssertions");
                file.AddUsing("System.Collections.Generic");
                file.AddUsing("System.Linq");

                var priClass = file.Classes.First();
                priClass.Static();
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