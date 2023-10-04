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
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class DbContextTemplate : CSharpTemplateBase<IList<ClassModel>, ITemplateDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.EntityFrameworkCore.DbContext";

        private readonly List<CapturedEntityTypeConfiguration> _capturedEntityTypeConfigurations = new();
        private bool _addedPostTypeConfigProcess;

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

                    if (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
                    {
                        @class.AddMethod(UseType("System.Threading.Tasks.Task"), "EnsureDbCreatedAsync", method =>
                        {
                            method.Async();
                            method.WithComments(@"
/// <summary>
/// Calling EnsureCreatedAsync is necessary to create the required containers and insert the seed data if present in the model. 
/// However EnsureCreatedAsync should only be called during deployment, not normal operation, as it may cause performance issues.
/// </summary>");
                            method.AddStatement("await Database.EnsureCreatedAsync();");
                        });
                    }
                });

            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(typeConfiguration =>
            {
                _capturedEntityTypeConfigurations.Add(new CapturedEntityTypeConfiguration
                {
                    Event = typeConfiguration,
                    DbSetElementType = GetEntityName(typeConfiguration.Template.Model),
                    DbSetName = GetEntityNameOnly(typeConfiguration.Template.Model).ToPascalCase().Pluralize(),
                    Prefix = string.Concat(((IHasFolder)typeConfiguration.Template.Model).GetParentFolderNames().Select(s => s.ToPascalCase()))
                });

                if (_addedPostTypeConfigProcess)
                {
                    return;
                }

                CSharpFile.AfterBuild(DoPostTypeConfigProcess);

                _addedPostTypeConfigProcess = true;
            });
        }

        // This is the cleanest (and foolproof way) I can get to ensure that DbSet name and types are unique since
        // all the EntityTypeConfigurations are made known via events so a buffer of sorts is
        // required to keep track of all the registered ones and then check if there are any duplicates.
        private void DoPostTypeConfigProcess(CSharpFile file)
        {
            var dbSetNameLookup = new Dictionary<string, CapturedEntityTypeConfiguration>();
            foreach (var entry in _capturedEntityTypeConfigurations)
            {
                if (dbSetNameLookup.TryAdd(entry.DbSetName, entry))
                {
                    continue;
                }

                var collisionEntry = dbSetNameLookup[entry.DbSetName];
                dbSetNameLookup.Remove(entry.DbSetName);

                entry.DbSetName = entry.Prefix + entry.DbSetName;
                collisionEntry.DbSetName = collisionEntry.Prefix + collisionEntry.DbSetName;

                dbSetNameLookup.Add(entry.DbSetName, entry);
                dbSetNameLookup.Add(collisionEntry.DbSetName, collisionEntry);
            }

            var dbSetElementTypeLookup = new Dictionary<string, CapturedEntityTypeConfiguration>();
            foreach (var entry in _capturedEntityTypeConfigurations)
            {
                if (dbSetElementTypeLookup.TryAdd(entry.DbSetElementType, entry))
                {
                    continue;
                }

                var collisionEntry = dbSetNameLookup[entry.DbSetElementType];
                dbSetElementTypeLookup.Remove(entry.DbSetElementType);

                // Ensure that the GetTypeName system picks up the duplicate and re-resolves them to a full type name
                entry.DbSetElementType = GetEntityName(entry.Event.Template.Model);
                collisionEntry.DbSetElementType = GetEntityName(collisionEntry.Event.Template.Model);

                dbSetElementTypeLookup.Add(entry.DbSetElementType, entry);
                dbSetElementTypeLookup.Add(collisionEntry.DbSetElementType, collisionEntry);
            }

            var @class = CSharpFile.Classes.First();
            foreach (var entry in _capturedEntityTypeConfigurations)
            {
                @class.AddProperty(
                    type: $"DbSet<{entry.DbSetElementType}>",
                    name: entry.DbSetName,
                    configure: prop => prop.AddMetadata("model", entry.Event.Template.Model));

                @class.Methods.First(x => x.Name.Equals("OnModelCreating"))
                    .AddStatement($"modelBuilder.ApplyConfiguration(new {GetTypeName(entry.Event.Template)}());",
                        config => config.AddMetadata("model", entry.Event.Template.Model));

                AddTemplateDependency(TemplateDependency.OnTemplate(entry.Event.Template)); // needed? GetTypeName does the same thing?
            }
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
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

        private class CapturedEntityTypeConfiguration
        {
            public EntityTypeConfigurationCreatedEvent Event { get; set; }
            public string DbSetElementType { get; set; }
            public string DbSetName { get; set; }
            public string Prefix { get; set; }
        }
    }
}