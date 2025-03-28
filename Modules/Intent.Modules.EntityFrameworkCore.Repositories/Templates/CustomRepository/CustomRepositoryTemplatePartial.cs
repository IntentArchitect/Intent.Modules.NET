#nullable enable
using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.CustomRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.CustomRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CustomRepositoryTemplate : CSharpTemplateBase<RepositoryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.CustomRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CustomRepositoryTemplate(IOutputTarget outputTarget, RepositoryModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name.EnsureSuffixedWith("Repository")}", @class =>
                {
                    @class.ImplementsInterface(this.GetCustomRepositoryInterfaceName());
                    @class.AddConstructor(ctor =>
                    {
                        if (!string.IsNullOrWhiteSpace(DbContextName))
                        {
                            ctor.AddParameter(DbContextName, "dbContext", p => p.IntroduceReadonlyField());
                        }
                    });

                    CustomRepositoryHelpers.ApplyImplementationMethods<CustomRepositoryTemplate, RepositoryModel>(
                        template: this,
                        repositoryModel: Model,
                        dbContextInstance: DbContextInstance);
                });
        }

        public override void BeforeTemplateExecution()
        {
            if (!TryGetTemplate<IClassProvider>(CustomRepositoryInterfaceTemplate.TemplateId, Model, out var contractTemplate))
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
        }

        private DbContextInstance DbContextInstance => new(Model.InternalElement.Package.AsDomainPackageModel());
        public string DbContextName => DbContextInstance.GetTypeName(this);

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