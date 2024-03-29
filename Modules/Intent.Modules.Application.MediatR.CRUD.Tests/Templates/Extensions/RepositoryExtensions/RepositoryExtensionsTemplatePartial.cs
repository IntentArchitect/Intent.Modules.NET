using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Extensions.RepositoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class RepositoryExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.Extensions.RepositoryExtensions";

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public RepositoryExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.NSubstitute);
        AddNugetDependency(NugetPackages.AutoFixture);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass($"RepositoryExtensions")
            .OnBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Linq.Expressions");
                file.AddUsing("System.Reflection");
                file.AddUsing("System.Threading");
                file.AddUsing("NSubstitute");
                file.AddUsing("AutoFixture");

                var priClass = file.Classes.First();
                priClass.Static();

                priClass.AddMethod("void", "OnAdd", method =>
                {
                    method.Static();
                    method.AddGenericParameter("TDomain", out var tDomain);
                    method.AddParameter($"{this.GetRepositoryInterfaceName()}<{tDomain}>", "repository", parm => parm.WithThisModifier());
                    method.AddParameter($"Action<{tDomain}>", "addAction");
                    method.AddStatement($"repository.When(x => x.Add(Arg.Any<{tDomain}>())).Do(ci => addAction(ci.Arg<{tDomain}>()));");
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