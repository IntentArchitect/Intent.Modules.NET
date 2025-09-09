using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            string nullableChar = OutputTarget.GetProject().NullableEnabled ? "?" : "";

            FulfillsRole(TemplateRoles.Repository.Implementation.Entity);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System")
                .AddClass($"{Model.Name}MongoRepository", @class =>
                {
                    // this will force a load of the Domain.Entities type which in turn means 
                    // entityDocumentName resolves with the full namespace if conflicts occur
                    _ = EntityTypeName;

                    var pkAttribute = Model.GetPrimaryKeyAttribute();
                    var pkFieldName = pkAttribute.Name.ToCamelCase();
                    var genericTypeParameters = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;
                    var entityDocumentName = $"{this.GetMongoDbDocumentName()}{genericTypeParameters}";
                    var entityDocumentInterfaceName = $"{this.GetMongoDbDocumentInterfaceName()}{genericTypeParameters}";

                    @class.Internal();
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    var entityStateGenericTypeArgument = createEntityInterfaces
                        ? $", {EntityStateTypeName}"
                        : string.Empty;

                    var pkType = GetPKType(pkAttribute);

                    @class.ExtendsClass($"{this.GetMongoDbRepositoryBaseName()}<{EntityTypeName}{entityStateGenericTypeArgument}, {entityDocumentName}, {entityDocumentInterfaceName}, {pkType}>");
                    @class.ImplementsInterface($"{this.GetEntityRepositoryInterfaceName()}{genericTypeParameters}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType("MongoDB.Driver.IMongoCollection")}<{entityDocumentName}>", "collection");
                        ctor.AddParameter(this.GetMongoDbUnitOfWorkName(), "unitOfWork");
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("collection")
                            .AddArgument("unitOfWork")
                        );
                    });

                    //var baseQualifier = pkType == "string"
                    //    ? "base."
                    //    : string.Empty;

                    //@class.AddMethod($"{UseType("System.Threading.Tasks.Task")}<{EntityTypeName}{nullableChar}>", "FindByIdAsync", method =>
                    //{
                    //    method
                    //        .Async()
                    //        .AddParameter(pkType, "id")
                    //        .AddOptionalCancellationTokenParameter(this)
                    //        .WithExpressionBody($"await {baseQualifier}FindAsync({GetPKUsage(pkAttribute)}, cancellationToken)");
                    //});

                    //@class.AddMethod($"{UseType("System.Threading.Tasks.Task")}<{UseType("System.Collections.Generic.List")}<{EntityTypeName}>>", "FindByIdsAsync", method =>
                    //{
                    //    AddUsing("System.Linq");
                    //    method
                    //        .Async()
                    //        .AddParameter($"{pkType}[]", "ids")
                    //        .AddOptionalCancellationTokenParameter(this)
                    //        .WithExpressionBody($"await {baseQualifier}FindAllAsync({GetPKUsages(pkAttribute)}, cancellationToken)");
                    //});

                    //@class.AddMethod($"{UseType("System.Collections.Generic.List")}<{EntityTypeName}>", "SearchText", method =>
                    //{
                    //    AddUsing("System.Linq");
                    //    method
                    //        .AddParameter($"string", "searchText")
                    //        .AddParameter($"Expression<Func<{EntityTypeName}, bool>>", "filterExpression")
                    //        .AddStatement($"throw new NotImplementedException();");
                    //});
                });
        }

        internal string GetPKType(AttributeModel pkAttribute)
        {
            if (pkAttribute.Id != pkAttribute.Id)
            {
                return $"({GetTypeName(pkAttribute)} {pkAttribute.Name.ToPascalCase()},{GetTypeName(pkAttribute)} {pkAttribute.Name.ToPascalCase()})";
            }

            return GetTypeName(pkAttribute);
        }

        private string GetPKUsage(AttributeModel pkAttribute)
        {
            string rowId = $"x.{pkAttribute.Name.ToPascalCase()}";
            return $"x => {rowId} == id";
        }

        private string GetPKUsages(AttributeModel pkAttribute)
        {
            string rowId = $"x.{pkAttribute.Name.ToPascalCase()}";
            return $"x => ids.Contains({rowId})";
        }

        public string GenericTypeParameters => Model.GenericTypes.Any()
            ? $"<{string.Join(", ", Model.GenericTypes)}>"
            : string.Empty;

        public string EntityTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{GenericTypeParameters}";
        public string EntityStateTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Primary, Model)}{GenericTypeParameters}";

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model, accessibleTo: null);
            if (contractTemplate == null)
            {
                return;
            }

            ((ICSharpFileBuilderTemplate)contractTemplate).CSharpFile.Interfaces[0].AddMetadata("requires-explicit-update", true);
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate)
                .WithPerServiceCallLifeTime()
            );
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