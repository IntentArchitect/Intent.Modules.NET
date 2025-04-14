using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapper.Templates.CustomRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates.CustomRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CustomRepositoryTemplate : CSharpTemplateBase<RepositoryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapper.CustomRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CustomRepositoryTemplate(IOutputTarget outputTarget, RepositoryModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name.EnsureSuffixedWith("Repository")}", @class =>
                {
                    @class.ImplementsInterface(this.GetCustomRepositoryInterfaceName());
                    @class.AddConstructor(ctor =>
                    {
                        //ctor.AddParameter(DbContextName, "dbContext", p => p.IntroduceReadonlyField());
                    });
                    RepositoryOperationHelper.ApplyMethods(this, @class, model);
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (!method.Statements.Any())
                        {
                            method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                            method.AddStatement($"""throw new {UseType("System.NotImplementedException")}("Your implementation here...");""");
                        }
                    }
                }, 1000);
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