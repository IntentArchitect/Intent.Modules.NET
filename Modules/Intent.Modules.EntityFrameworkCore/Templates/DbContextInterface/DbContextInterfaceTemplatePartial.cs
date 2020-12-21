using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface
{
    partial class DbContextInterfaceTemplate : CSharpTemplateBase<IEnumerable<ClassModel>>, ITemplateBeforeExecutionHook, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbContextInterface";


        public DbContextInterfaceTemplate(IEnumerable<ClassModel> models, IOutputTarget outputTarget)
            : base(Identifier, outputTarget, models)
        {
            AddNugetDependency(NugetPackages.EntityFrameworkCore);
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName(DomainEntityStateTemplate.Identifier, model);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Project.Application.Name}DbContext".ToCSharpIdentifier(),
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetMappingClassName(ClassModel model)
        {
            return GetTypeName(EFMapping.EFMappingTemplate.Identifier, model);
        }
    }
}
