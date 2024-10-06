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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Ardalis.Repositories.Templates.Specification
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SpecificationTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Ardalis.Repositories.Specification";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SpecificationTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Domain.Specification);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Ardalis.Specification")
                .AddClass($"{Model.Name}Spec", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    @class.WithBaseType($"Specification<{GetPersistenceEntityTypeName()}>");
                    @class.AddConstructor(ctor => 
                    {
                        @ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });
                }).AfterBuild(file => 
                { 
                    var @class = file.Classes.FirstOrDefault();
                    var pk = GetSurrogateKey();
                    if (pk != null)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter(UseType(pk.Type), pk.Name.ToCamelCase());
                            ctor.AddStatement($"Query.Where(x => x.{pk.Name} == {pk.Name.ToCamelCase()});\r\n");
                        });
                    }
                });
        }

        private string GetPersistenceEntityTypeName()
        {
            return GetTypeName("Domain.Entity", Model);
        }

        private CSharpProperty GetSurrogateKey()
        {
            if (!TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model, out var entityTemplate))
            {
                return null;
            }

            return entityTemplate.CSharpFile.Classes.First().GetPropertyWithPrimaryKey();
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