using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Events;
using Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DbContextTemplate : CSharpTemplateBase<IList<ClassModel>, DbContextDecoratorBase>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DbContext";

        private readonly IList<EntityTypeConfigurationCreatedEvent> _entityTypeConfigurations = new List<EntityTypeConfigurationCreatedEvent>();
        private readonly IList<DbContextDecoratorBase> _decorators = new List<DbContextDecoratorBase>();
        private bool _useDbContextAsOptionsParameter;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DbContextTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<OverrideDbContextOptionsEvent>(evt =>
            {
                _useDbContextAsOptionsParameter |= evt.UseDbContextAsOptionsParameter;
            });
            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(evt =>
            {
                _entityTypeConfigurations.Add(evt);
                AddTemplateDependency(TemplateDependency.OnTemplate(evt.Template));
            });
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ApplicationDbContext",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName(GetMetadata().CustomMetadata["Entity Template Id"], model);
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return (UseLazyLoadingProxies
                ? new[]
                {
                    NugetPackages.EntityFrameworkCore(Project),
                    NugetPackages.EntityFrameworkCoreProxies(Project),
                }
                : new[]
                {
                    NugetPackages.EntityFrameworkCore(Project),
                })
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public bool UseLazyLoadingProxies => !bool.TryParse(GetMetadata().CustomMetadata["Use Lazy-Loading Proxies"], out var useLazyLoadingProxies) || useLazyLoadingProxies;


        public string GetMethods()
        {
            var code = string.Join(Environment.NewLine + Environment.NewLine,
                GetDecorators()
                    .SelectMany(s => s.GetMethods())
                    .Where(p => !string.IsNullOrEmpty(p)));
            if (string.IsNullOrWhiteSpace(code))
            {
                return string.Empty;
            }
            return Environment.NewLine + Environment.NewLine + code;
        }

        public string GetBaseTypes()
        {
            try
            {
                var baseClass = NormalizeNamespace(GetDecorators().Select(x => x.GetBaseClass()).SingleOrDefault(x => x != null) ?? "Microsoft.EntityFrameworkCore.DbContext");
                return $"{baseClass}, {GetTypeName(DbContextInterfaceTemplate.Identifier)}";
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Multiple decorators attempting to modify 'base class' on {TemplateId}");
            }
        }

        public string GetDbContextOptionsType()
        {
            return $"DbContextOptions<{ClassName}>";
            //if (_useDbContextAsOptionsParameter)
            //{
            //    return $"DbContextOptions<{ClassName}>";
            //}
            //return "DbContextOptions";
        }

        public string GetMappingClassName(ClassModel model)
        {
            return GetTypeName(EntityTypeConfigurationTemplate.TemplateId, model);
        }

        private string GetPrivateFields()
        {
            var privateFields = GetDecorators().SelectMany(x => x.GetPrivateFields() ?? Enumerable.Empty<string>()).ToList();
            return privateFields.Any() ? string.Join(@"
        ", privateFields) + @"
        " : "";
        }

        private string GetConstructorParameters()
        {
            var privateFields = GetDecorators().SelectMany(x => x.GetConstructorParameters() ?? Enumerable.Empty<string>()).ToList();
            return privateFields.Any() ? @",
            " + string.Join(@",
            ", privateFields) : "";
        }

        private string GetConstructorInitializations()
        {
            var privateFields = GetDecorators().SelectMany(x => x.GetConstructorInitializations() ?? Enumerable.Empty<string>()).ToList();
            return privateFields.Any() ? @"
            " + string.Join(@"
            ", privateFields) : "";
        }
    }
}
