using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    public partial class DomainEntityStateTemplate : DomainEntityStateTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntityState";
        public const string InterfaceContext = "Interface";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEntityStateTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Domain.Entity.Primary);
            if (!ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
            {
                FulfillsRole(TemplateRoles.Domain.Entity.Interface);
            }

            AddTypeSource(TemplateRoles.Domain.ValueObject);
            AddTypeSource(TemplateRoles.Domain.Entity.Interface);
            AddTypeSource(TemplateRoles.Domain.DomainServices.Interface);
            AddTypeSource(TemplateRoles.Domain.DataContract);
            AddTypeSource(DomainEnumTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass(Model.Name, @class =>
                {
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    @class.AddMetadata("model", Model);
                    @class.Partial();
                    @class.WithPropertiesSeparated();

                    if (Model.ParentClass != null)
                    {
                        var baseType = this.GetDomainEntityName(Model.ParentClass);
                        if (Model.ParentClassTypeReference.GenericTypeParameters.Any())
                        {
                            baseType = $"{baseType}<{string.Join(", ", Model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName))}>";
                        }

                        @class.ExtendsClass(baseType);
                    }

                    if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                    {
                        var domainEntityInterfaceName = this.GetDomainEntityInterfaceName();
                        if (Model.GenericTypes.Any())
                        {
                            domainEntityInterfaceName = $"{domainEntityInterfaceName}<{string.Join(", ", Model.GenericTypes)}>";
                        }

                        @class.ImplementsInterface(domainEntityInterfaceName);
                    }

                    AddProperties(@class);

                    foreach (var operation in Model.Operations)
                    {
                        if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces() &&
                            (!InterfaceTemplate.GetOperationTypeName(operation).Equals(this.GetOperationTypeName(operation)) ||
                             !operation.Parameters.Select(InterfaceTemplate.GetOperationTypeName).SequenceEqual(operation.Parameters.Select(this.GetOperationTypeName))))
                        {
                            AddInterfaceQualifiedMethod(@class, operation);
                        }
                    }
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            var config = CSharpFile.GetConfig();
            config.FileName = $"{Model.Name}State";
            return config;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
