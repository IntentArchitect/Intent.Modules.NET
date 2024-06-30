using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge)]
    public partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.Repository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryTemplate(IOutputTarget outputTarget, ClassModel model)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddClass($"{Model.Name}Repository", @class =>
                {
                    @class.AddMetadata("model", model);
                    @class.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                    @class.WithBaseType($"RepositoryBase<{EntityInterfaceName}, {EntityName}, {DbContextName}>");
                    @class.ImplementsInterface(RepositoryContractName);
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(DbContextName, "dbContext")
                            .CallsBase(b => b.AddArgument("dbContext"));
                        if (!string.IsNullOrWhiteSpace(ConstructorImplementation()))
                        {
                            ctor.AddStatement(ConstructorImplementation());
                        }
                    });

                    var interfaceTemplate = GetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
                    interfaceTemplate.CSharpFile.AfterBuild(file =>
                    {
                        var @interface = file.Interfaces.Single();
                        if (@interface.Interfaces.Count != 1)
                        {
                            Logging.Log.Warning("Could not change to extend EF interface as non-single count of extended interfaces found.");
                            return;
                        }

                        @interface.Interfaces.Clear();
                        @interface.ExtendsInterface($"{this.GetEFRepositoryInterfaceName()}<{EntityInterfaceName}, {EntityName}>");
                    });

                    if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model, out var entityTemplate))
                    {
                        entityTemplate.CSharpFile.AfterBuild(file =>
                        {
                            var rootEntity = file.Classes.First().GetRootEntity();
                            if (rootEntity.HasPrimaryKey())
                            {
                                AddMethods(@class, entityTemplate, rootEntity, true);
                                if (ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                                {
                                    AddMethods(@class, entityTemplate, rootEntity, makeAsync: false, makefully: true);
                                }
                            }
                            else
                            {
                                var parameter = @class.Constructors.First().Parameters.Single(x => x.Name == "dbContext");
                                parameter.IntroduceReadonlyField();
                                CSharpFile.AddUsing("Microsoft.EntityFrameworkCore");

                                @class.AddMethod("void", "Add", method =>
                                {
                                    method.AddParameter(GetTypeName(TemplateRoles.Domain.Entity.Interface, Model), "entity");

                                    var columns = Model.Attributes.Select(x => x.Name.ToPascalCase());
                                    var values = Model.Attributes.Select(x => $"{{entity.{x.Name.ToPascalCase()}}}");

                                    method.AddStatement($"_dbContext.Database.ExecuteSqlInterpolated($\"INSERT INTO {Model.Name.Pluralize()} ({string.Join(", ", columns)}) VALUES({string.Join(", ", values)})\");");
                                });
                            }
                        });
                    }
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (!method.Statements.Any())
                        {
                            method.AddStatement($"// TODO: Implement {method.Name} ({file.Classes.First().Name}) functionality");
                            method.AddStatement($"""throw new {UseType("System.NotImplementedException")}("Your implementation here...");""");
                        }
                    }
                }, 1000);
        }

        private void AddMethods(CSharpClass @class, ICSharpFileBuilderTemplate entityTemplate, CSharpClass rootEntity, bool makeAsync, bool makefully = false)
        {
            var pks = rootEntity.GetPropertiesWithPrimaryKey();
            string returnType = $"{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{(OutputTarget.GetProject().NullableEnabled ? "?" : "")}";
            string methodName = makeAsync ? "FindByIdAsync" : "FindById";
            @class.AddMethod(makeAsync ? $"Task<{returnType}>" : returnType, methodName, method =>
            {
                if (makeAsync)
                {
                    method.Async();
                }
                if (makefully)
                {
                    method.AddAttribute("[IntentManaged(Mode.Fully)]");
                }
                if (pks.Length == 1)
                {
                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                    method.AddParameter(entityTemplate.UseType(pk.Type), pk.Name.ToCamelCase());
                    if (makeAsync)
                    {
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                    }
                    string findMethod = makeAsync ? "await FindAsync" : "Find";
                    method.AddStatement($"return {findMethod}(x => x.{pk.Name} == {pk.Name.ToCamelCase()}{(makeAsync ? ", cancellationToken" : "")});");
                }
                else
                {
                    method.AddParameter($"({string.Join(", ", pks.Select(pk => $"{entityTemplate.UseType(pk.Type)} {pk.Name.ToPascalCase()}"))})", "id");
                    if (makeAsync)
                    {
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                    }
                    string findMethod = makeAsync ? "await FindAsync" : "Find";
                    method.AddStatement($"return {findMethod}(x => {string.Join(" && ", pks.Select(pk => $"x.{pk.Name} == id.{pk.Name.ToPascalCase()}"))}{(makeAsync ? ", cancellationToken" : "")});");
                }
            });

            if (pks.Length == 1)
            {
                returnType = $"List<{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}>";
                methodName = makeAsync ? "FindByIdsAsync" : "FindByIds";
                @class.AddMethod(makeAsync ? $"Task<{returnType}>" : returnType, methodName, method =>
                {
                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                    if (makeAsync)
                    {
                        method.Async();
                    }
                    if (makefully)
                    {
                        method.AddAttribute("[IntentManaged(Mode.Fully)]");
                    }
                    method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                    if (makeAsync)
                    {
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                    }

                    string findMethod = makeAsync ? "await FindAllAsync" : "FindAll";
                    method.AddStatement($"return {findMethod}(x => {pk.Name.ToCamelCase().Pluralize()}.Contains(x.{pk.Name}){(makeAsync ? ", cancellationToken" : "")});");
                });
            }

        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Repository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public string EntityName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public string RepositoryContractName => TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, Model) ?? $"I{ClassName}";

        public string PrimaryKeyType => GetTemplate<ITemplate>("Domain.Entity", Model).GetMetadata().CustomMetadata.TryGetValue("Surrogate Key Type", out var type) ? UseType(type) : UseType("System.Guid");

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id";

        public string DbContextName
        {
            get
            {
                var dbContextInstance = DbContextManager.GetDbContext(Model);
                return dbContextInstance.GetTypeName(this);
            }
        }

        public override void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
        }

        private string ConstructorImplementation()
        {
            if (!IsRepoSupported())
            {
                return $@"
            // The {Model.Name} has no EntityFrameworkCore type configuration associated with it.
            // Add the 'Table' stereotype to this entity in the Domain designer.
            throw new NotSupportedException($""Cannot create a repository for type {Model.Name}."");";
            }

            return string.Empty;
        }

        private bool IsRepoSupported()
        {
            return TryGetTemplate<EntityTypeConfigurationTemplate>(EntityTypeConfigurationTemplate.TemplateId, Model.Id, out var _);
        }

    }
}
