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

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.RepositoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RepositoryExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.RepositoryExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.NSubstitute);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"RepositoryExtensions")
                .OnBuild(file =>
                {
                    file.AddUsing("System");
                    file.AddUsing("System.Threading");
                    file.AddUsing("NSubstitute");

                    var priClass = file.Classes.First();
                    priClass.Static();

                    priClass.AddMethod("void", "OnAdd", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TDomain", out var TDomain);
                        method.AddGenericParameter("TPersistence", out var TPersistence);
                        method.AddParameter($"{this.GetRepositoryInterfaceName()}<{TDomain}, {TPersistence}>", "repository", parm => parm.WithThisModifier());
                        method.AddParameter($"Action<{TDomain}>", "addAction");
                        method.AddStatement($"repository.When(x => x.Add(Arg.Any<{TDomain}>())).Do(ci => addAction(ci.Arg<{TDomain}>()));");
                    });

                    priClass.AddMethod("void", "OnSave", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TDomain", out var TDomain);
                        method.AddGenericParameter("TPersistence", out var TPersistence);
                        method.AddParameter($"{this.GetRepositoryInterfaceName()}<{TDomain}, {TPersistence}>", "repository", parm => parm.WithThisModifier());
                        method.AddParameter($"Action", "saveAction");
                        method.AddStatement($"repository.UnitOfWork.When(async x => await x.SaveChangesAsync(CancellationToken.None)).Do(_ => saveAction());");
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