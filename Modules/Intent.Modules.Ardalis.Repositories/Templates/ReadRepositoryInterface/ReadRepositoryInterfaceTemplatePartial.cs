using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Ardalis.Repositories.Templates.ReadRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ReadRepositoryInterfaceTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Ardalis.Repositories.ReadRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ReadRepositoryInterfaceTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.ArdalisSpecification(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"I{Model.Name.ToPascalCase()}ReadRepository", @interface =>
                {
                    @interface.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @interface.ImplementsInterfaces($"IReadRepositoryBase<{ GetPersistenceEntityTypeName()}>");
                }).AfterBuild(file => 
                {
                    var @interface = file.Interfaces.First();
                    if (HasSinglePrimaryKey())
                    {
                        @interface.AddMethod($"Task<{GetDomainEntityTypeName()}?>", "FindByIdAsync", method =>
                        {
                            method
                                .AddAttribute(CSharpIntentManagedAttribute.Fully())
                                .AddParameter(GetSurrogateKey(), "id")
                                .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        });
                    }
                }
                );
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name.ToPascalCase()}ReadRepository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetPersistenceEntityTypeName()
        {
            return GetTypeName("Domain.Entity", Model);
        }

        private string GetDomainEntityTypeName()
        {
            return GetTypeName(TemplateRoles.Domain.Entity.Interface, Model);
        }

        private bool HasSinglePrimaryKey()
        {
            if (!TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model, out var entityTemplate))
            {
                return false;
            }

            return entityTemplate.CSharpFile.Classes.First().HasSinglePrimaryKey();
        }

        private string GetSurrogateKey()
        {
            if (!TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model, out var entityTemplate))
            {
                return string.Empty;
            }

            return UseType(entityTemplate.CSharpFile.Classes.First().GetPropertyWithPrimaryKey().Type);
        }
    }
}