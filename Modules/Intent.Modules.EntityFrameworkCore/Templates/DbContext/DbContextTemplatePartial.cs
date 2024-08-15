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
using Intent.Modules.EntityFrameworkCore.Helpers;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

#nullable enable

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DbContextTemplate : CSharpTemplateBase<DbContextInstance>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.EntityFrameworkCore.DbContext";

        private readonly Lazy<DbContextInterfaceTemplate> _interfaceTemplate;
        private readonly List<CapturedEntityTypeConfiguration> _capturedEntityTypeConfigurations = new();
        private bool _addedPostTypeConfigProcess;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore)]
        public DbContextTemplate(IOutputTarget outputTarget, DbContextInstance model) : base(TemplateId, outputTarget, model)
        {
            _interfaceTemplate = new Lazy<DbContextInterfaceTemplate>(() => GetTemplate<DbContextInterfaceTemplate>(
                templateId: DbContextInterfaceTemplate.TemplateId,
                model: model,
                options: new TemplateDiscoveryOptions
                {
                    TrackDependency = false
                }));

            if (Model.IsApplicationDbContext)
            {
                FulfillsRole(TemplateRoles.Infrastructure.Data.DbContext);
            }

            FulfillsRole(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext);

            var currentDatabaseProvider = DbContextManager.GetDatabaseProviderForDbContext(Model.DbProvider, ExecutionContext);

            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddClass(Model.DbContextName, @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.EntityFrameworkCore.DbContext"));
                    @class.ImplementsInterfaces(GetInterfaces());
                    @class.AddConstructor(ctor => { ctor.AddParameter(GetDbContextOptionsType(), "options", param => { ctor.CallsBase(call => call.AddArgument(param.Name)); }); });

                    @class.AddMethod("void", "OnModelCreating", method =>
                    {
                        method.Protected().Override()
                            .AddParameter("ModelBuilder", "modelBuilder");

                        method.AddStatement("base.OnModelCreating(modelBuilder);");
                        if (Model.IsApplicationDbContext && !string.IsNullOrWhiteSpace(ExecutionContext.Settings.GetDatabaseSettings().DefaultSchemaName()))
                        {
                            method.AddStatement($@"modelBuilder.HasDefaultSchema(""{ExecutionContext.Settings.GetDatabaseSettings().DefaultSchemaName()}"");");
                        }

                        if (currentDatabaseProvider == DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql && NetTopologySuiteHelper.IsInstalled(ExecutionContext))
                        {
                            method.AddStatement(@"modelBuilder.HasPostgresExtension(""postgis"");");
                        }

                        method.AddStatement("ConfigureModel(modelBuilder);", s => s.SeparatedFromPrevious());
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
    new Car() { CarId = 3, Make = ""Lamborghini"", Model = ""Countach"" });
*/");
                    });

                    if (currentDatabaseProvider == DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos)
                    {
                        @class.AddMethod(UseType("System.Threading.Tasks.Task"), "EnsureDbCreatedAsync", method =>
                        {
                            method.Async();
                            method.WithComments("""
                                                /// <summary>
                                                /// Calling EnsureCreatedAsync is necessary to create the required containers and insert the seed data if present in the model.
                                                /// However EnsureCreatedAsync should only be called during deployment, not normal operation, as it may cause performance issues.
                                                /// </summary>
                                                """);
                            method.AddStatement("await Database.EnsureCreatedAsync();");
                        });
                    }
                });

            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(typeConfiguration =>
            {
                if (!Model.Equals(DbContextManager.GetDbContext(typeConfiguration.Template.Model)))
                {
                    return;
                }

                _capturedEntityTypeConfigurations.Add(new CapturedEntityTypeConfiguration
                {
                    Event = typeConfiguration,
                    DbSetElementType = GetEntityName(this, typeConfiguration.Template.Model),
                    InterfaceDbSetElementType = _interfaceTemplate.Value.IsEnabled
                        ? GetEntityName(_interfaceTemplate.Value, typeConfiguration.Template.Model)
                        : null,
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

        private static void MakeDbSetNameUnique(CapturedEntityTypeConfiguration lhs, CapturedEntityTypeConfiguration rhs)
        {
            if (lhs.Prefix != rhs.Prefix)
            {
                lhs.DbSetName = lhs.Prefix + lhs.DbSetName;
                rhs.DbSetName = rhs.Prefix + rhs.DbSetName;
                return;
            }

            if (lhs.DbSetElementType != rhs.DbSetElementType)
            {
                lhs.DbSetName = lhs.DbSetElementType;
                rhs.DbSetName = rhs.DbSetElementType;
                return;
            }

            throw new Exception($"Different Entities resolving to same DB Set name `{lhs.DbSetName}` for {lhs.DbSetElementType} and {rhs.DbSetName}");
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

                MakeDbSetNameUnique(entry, collisionEntry);

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
                entry.DbSetElementType = GetEntityName(this, entry.Event.Template.Model);
                collisionEntry.DbSetElementType = GetEntityName(this, collisionEntry.Event.Template.Model);
                if (_interfaceTemplate.Value.IsEnabled)
                {
                    entry.InterfaceDbSetElementType = GetEntityName(_interfaceTemplate.Value, entry.Event.Template.Model);
                    collisionEntry.InterfaceDbSetElementType = GetEntityName(_interfaceTemplate.Value, collisionEntry.Event.Template.Model);
                }

                dbSetElementTypeLookup.Add(entry.DbSetElementType, entry);
                dbSetElementTypeLookup.Add(collisionEntry.DbSetElementType, collisionEntry);
            }

            var @class = CSharpFile.Classes.First();
            var @interface = _interfaceTemplate.Value.CSharpFile.Interfaces.First();

            foreach (var entry in _capturedEntityTypeConfigurations)
            {
                @class.AddProperty(
                    type: $"DbSet<{entry.DbSetElementType}>",
                    name: entry.DbSetName,
                    configure: prop => prop.AddMetadata("model", entry.Event.Template.Model));

                if (_interfaceTemplate.Value.IsEnabled)
                {
                    @interface.AddProperty(
                        type: $"DbSet<{entry.InterfaceDbSetElementType}>",
                        name: entry.DbSetName,
                        configure: prop => prop.WithoutSetter());
                }

                @class.Methods.First(x => x.Name.Equals("OnModelCreating"))
                    .AddStatement($"modelBuilder.ApplyConfiguration(new {GetTypeName(entry.Event.Template)}());",
                        config => config.AddMetadata("model", entry.Event.Template.Model));

                AddTemplateDependency(TemplateDependency.OnTemplate(entry.Event.Template)); // needed? GetTypeName does the same thing?
            }
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        public override void BeforeTemplateExecution()
        {
            if (_interfaceTemplate.Value.IsEnabled)
            {
                AddTemplateDependency(_interfaceTemplate.Value.Id);

                ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                    .ToRegister(this)
                    .ForInterface(_interfaceTemplate.Value)
                    .ForConcern("Infrastructure")
                    .WithResolveFromContainer()
                    .WithPerServiceCallLifeTime());
            }

            base.BeforeTemplateExecution();
        }

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

        private static string GetEntityName(IIntentTemplate template, ClassModel model)
        {
            return template.GetTypeName("Domain.Entity", model);
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
                        NugetPackages.MicrosoftEntityFrameworkCore(Project),
                        NugetPackages.MicrosoftEntityFrameworkCoreProxies(Project),
                    }
                    : new[]
                    {
                        NugetPackages.MicrosoftEntityFrameworkCore(Project),
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
                if (_interfaceTemplate.Value.IsEnabled)
                {
                    interfaces.Add(GetTypeName(TemplateRoles.Application.Common.ConnectionStringDbContextInterface, Model));
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
            public string? InterfaceDbSetElementType { get; set; }
        }
    }
}