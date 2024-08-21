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
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.Templates.Repository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.Repositories.Repository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}MongoRepository", @class =>
                {
                    var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
                    if (createEntityInterfaces)
                    {
                        @class.WithBaseType($"{this.GetMongoRepositoryBaseName()}<{EntityInterfaceName}, {EntityName}>");
                    }
                    else
                    {
                        @class.WithBaseType($"{this.GetMongoRepositoryBaseName()}<{EntityName}>");
                    }

                    @class.ImplementsInterface(RepositoryContractName);
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(DbContextName, "context");
                        ctor.CallsBase(b => b.AddArgument("context"));
                    });

                    var interfaceTemplate = GetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
                    interfaceTemplate.CSharpFile.AfterBuild(file =>
                    {
                        var @interface = file.Interfaces.Single();
                        if (@interface.Interfaces.Count != 1)
                        {
                            Logging.Log.Warning("Could not change to extend MongoDb interface as non-single count of extended interfaces found.");
                            return;
                        }

                        @interface.Interfaces.Clear();
                        if (createEntityInterfaces)
                        {
                            @interface.ExtendsInterface($"{this.GetMongoRepositoryInterfaceName()}<{EntityInterfaceName}, {EntityName}>");
                        }
                        else
                        {
                            @interface.ExtendsInterface($"{this.GetMongoRepositoryInterfaceName()}<{EntityName}>");
                        }
                    });

                    if (TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", Model, out var entityTemplate))
                    {
                        entityTemplate.CSharpFile.AfterBuild(file =>
                        {
                            var rootEntity = file.Classes.First().GetRootEntity();
                            if (!rootEntity.HasSinglePrimaryKey())
                            {
                                @class.AddMethod("void", "Remove", method =>
                                {
                                    method.Override();
                                    method.AddParameter(EntityInterfaceName, "entity");
                                    method.AddStatement($@"throw NotImplementedException(""Composite Keys not supported and needs to be implemented manually"");");
                                });
                            }
                            else
                            {
                                @class.AddMethod($"Task<{GetTypeName("Domain.Entity.Interface", Model)}?>", "FindByIdAsync", method =>
                                {
                                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                                    method.Async();
                                    method.AddParameter(entityTemplate.UseType(pk.Type), pk.Name.ToCamelCase());
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                                    method.AddStatement($"return await FindAsync(x => x.{pk.Name} == {pk.Name.ToCamelCase()}, cancellationToken);");
                                });
                                @class.AddMethod($"Task<List<{GetTypeName("Domain.Entity.Interface", Model)}>>", "FindByIdsAsync", method =>
                                {
                                    var pk = rootEntity.GetPropertyWithPrimaryKey();
                                    method.Async();
                                    method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                                    method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                                    method.AddStatement($"return await FindAllAsync(x => {pk.Name.ToCamelCase().Pluralize()}.Contains(x.{pk.Name}), cancellationToken);");
                                });
                            }
                        });
                    }
                });
        }

        public string EntityName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public string RepositoryContractName => TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, Model, out var className) ? className : $"I{ClassName}";
        public string DbContextName => TryGetTypeName("Infrastructure.Data.DbContext.MongoDb", out var dbContextName) ? dbContextName : $"{Model.Application.Name}MongoDbContext";


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

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }
            ((ICSharpFileBuilderTemplate)contractTemplate).CSharpFile.Interfaces[0].AddMetadata("requires-explicit-update", true);

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
        }
    }
}