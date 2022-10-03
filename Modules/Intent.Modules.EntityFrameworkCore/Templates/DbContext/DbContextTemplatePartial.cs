using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
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
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.EntityFrameworkCore.DbContext";

        private readonly IList<EntityTypeConfigurationCreatedEvent> _entityTypeConfigurations = new List<EntityTypeConfigurationCreatedEvent>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DbContextTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole("Infrastructure.Data.DbContext");

            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(evt =>
            {
                _entityTypeConfigurations.Add(evt);
                AddTemplateDependency(TemplateDependency.OnTemplate(evt.Template));
            });
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var decorator in GetDecorators())
            {
                decorator.OnBeforeTemplateExecution();
            }

            if (!TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWorkInterface))
            {
                ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                    .ToRegister(this)
                    .ForInterface(GetTemplate<IClassProvider>(DbContextInterfaceTemplate.TemplateId))
                    .ForConcern("Infrastructure")
                    .WithResolveFromContainer()
                    .WithPerServiceCallLifeTime());
            }

            base.BeforeTemplateExecution();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ApplicationDbContext",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName("Domain.Entity", model);
        }
        
        public string GetEntityNameOnly(ClassModel model)
        {
            var typeInfo = this.GetTypeInfo("Domain.Entity", model);
            return typeInfo.Name;
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

        public bool UseLazyLoadingProxies =>
            !bool.TryParse(GetMetadata().CustomMetadata["Use Lazy-Loading Proxies"], out var useLazyLoadingProxies) || useLazyLoadingProxies;

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
                var baseTypes = new List<string>();
                baseTypes.Add(UseType(GetDecorators().Select(x => x.GetBaseClass()).SingleOrDefault(x => x != null) ??
                                      "Microsoft.EntityFrameworkCore.DbContext"));
                if (TryGetTypeName(DbContextInterfaceTemplate.TemplateId, out var dbContextInterface))
                {
                    baseTypes.Add(dbContextInterface);
                }

                baseTypes.AddRange(GetDecorators().Select(x => x.GetBaseInterfaces()).Where(x => x != null));
                return string.Join(", ", baseTypes);
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Multiple decorators attempting to modify 'base class' on {TemplateId}");
            }
        }

        public string GetDbContextOptionsType()
        {
            return $"DbContextOptions<{ClassName}>";
        }

        public string GetMappingClassName(ClassModel model)
        {
            return GetTypeName(EntityTypeConfigurationTemplate.TemplateId, model);
        }

        private string GetPrivateFields()
        {
            var privateFields = GetDecorators().SelectMany(x => x.GetPrivateFields() ?? Enumerable.Empty<string>()).ToList();
            return privateFields.Any()
                ? string.Join(@"
        ", privateFields) + @"
        "
                : "";
        }

        private string GetConstructorParameters()
        {
            var privateFields = GetDecorators().SelectMany(x => x.GetConstructorParameters() ?? Enumerable.Empty<string>()).ToList();
            return privateFields.Any()
                ? @",
            " + string.Join(@",
            ", privateFields)
                : "";
        }

        private string GetConstructorInitializations()
        {
            var privateFields = GetDecorators().SelectMany(x => x.GetConstructorInitializations() ?? Enumerable.Empty<string>()).ToList();
            return privateFields.Any()
                ? @"
            " + string.Join(@"
            ", privateFields)
                : "";
        }

        private string GetOnModelCreatingStatements()
        {
            var statements = GetDecorators().SelectMany(x => x.GetOnModelCreatingStatements() ?? Enumerable.Empty<string>()).ToList();
            const string newLine = @"
            ";
            return string.Join(newLine, statements);
        }

        private string GetTypeConfigurationParameters(EntityTypeConfigurationCreatedEvent @event)
        {
            var parameters = GetDecorators().SelectMany(s => s.GetTypeConfigurationParameters(@event));
            return string.Join(",", parameters);
        }
    }
}