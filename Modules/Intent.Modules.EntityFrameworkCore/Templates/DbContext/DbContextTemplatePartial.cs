using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
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
    public partial class DbContextTemplate : CSharpTemplateBase<IList<ClassModel>, ITemplateDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.EntityFrameworkCore.DbContext";

        private readonly IList<EntityTypeConfigurationCreatedEvent> _entityTypeConfigurations = new List<EntityTypeConfigurationCreatedEvent>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DbContextTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole("Infrastructure.Data.DbContext");

            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddClass("ApplicationDbContext", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.EntityFrameworkCore.DbContext"));
                    @class.ImplementsInterfaces(GetInterfaces());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(GetDbContextOptionsType(), "options", param =>
                        {
                            ctor.CallsBase(call => call.AddArgument(param.Name));
                        });
                    });

                    @class.AddMethod("void", "OnModelCreating", method =>
                    {
                        method.Protected().Override()
                            .AddParameter("ModelBuilder", "modelBuilder");

                        method.AddStatement("base.OnModelCreating(modelBuilder);");
                        method.AddStatement("ConfigureModel(modelBuilder);", s => s.SeparatedFromPrevious());

                        //method.AddStatements(GetOnModelCreatingStatements());
                    });

                    @class.AddMethod("void", "ConfigureModel", method =>
                    {
                        method.Private()
                            .AddAttribute("IntentManaged", attr => attr.AddArgument("Mode.Ignore"))
                            .AddParameter("ModelBuilder", "modelBuilder")
                            .AddStatements(@"
// Seed data
// https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
/* Eg.

modelBuilder.Entity<Car>().HasData(
    new Car() { CarId = 1, Make = ""Ferrari"", Model = ""F40"" },
    new Car() { CarId = 2, Make = ""Ferrari"", Model = ""F50"" },
    new Car() { CarId = 3, Make = ""Labourghini"", Model = ""Countach"" });
*/");
                    });
                });

            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(typeConfiguration =>
            {
                _entityTypeConfigurations.Add(typeConfiguration);
                var @class = CSharpFile.Classes.First();
            
                @class.AddProperty($"DbSet<{GetEntityName(typeConfiguration.Template.Model)}>", GetEntityName(typeConfiguration.Template.Model).ToPluralName());
                
                @class.Methods.Single(x => x.Name.Equals("OnModelCreating"))
                    .AddStatement($"modelBuilder.ApplyConfiguration(new {GetTypeName(typeConfiguration.Template)}());");

                AddTemplateDependency(TemplateDependency.OnTemplate(typeConfiguration.Template)); // needed? GetTypeName does the same thing?
            });
        }
        public CSharpFile CSharpFile { get; }

        public override void BeforeTemplateExecution()
        {
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

        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName("Domain.Entity", model);
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
        
        public IEnumerable<string> GetInterfaces()
        {
            try
            {
                var interfaces = new List<string>();
                if (TryGetTypeName(DbContextInterfaceTemplate.TemplateId, out var dbContextInterface))
                {
                    interfaces.Add(dbContextInterface);
                }

                return interfaces;
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
    }
}