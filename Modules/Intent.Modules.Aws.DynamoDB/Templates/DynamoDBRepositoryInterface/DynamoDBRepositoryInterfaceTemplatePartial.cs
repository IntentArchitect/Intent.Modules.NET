using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBRepositoryInterfaceTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBRepositoryInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface("IDynamoDBRepository", @interface =>
                {
                    @interface
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPartitionKey", out var tPartitionKey, p => p.Contravariant());

                    @interface
                        .ImplementsInterfaces($"{this.GetRepositoryInterfaceName()}<{tDomain}>")
                        .AddProperty(this.GetDynamoDBUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                            .WithoutSetter()
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindByKeyAsync", method => method
                            .AddParameter(tPartitionKey, "partitionKey")
                            .AddOptionalCancellationTokenParameter()
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindByKeysAsync", method => method
                            .AddParameter($"IEnumerable<{tPartitionKey}>", "partitionKeys")
                            .AddOptionalCancellationTokenParameter()
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindByIdAsync", method => method
                            .AddParameter(tPartitionKey, "id")
                            .AddOptionalCancellationTokenParameter()
                            .WithExpressionBody("FindByKeyAsync(id, cancellationToken)")
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", method => method
                            .AddParameter($"IEnumerable<{tPartitionKey}>", "ids")
                            .AddOptionalCancellationTokenParameter()
                            .WithExpressionBody("FindByKeysAsync(ids, cancellationToken)")
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddOptionalCancellationTokenParameter()
                        )
                        ;
                })
                .AddInterface("IDynamoDBRepository", @interface =>
                {
                    @interface
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPartitionKey", out var tPartitionKey)
                        .AddGenericParameter("TSortKey", out var tSortKey);

                    @interface
                        .ImplementsInterfaces($"{this.GetRepositoryInterfaceName()}<{tDomain}>")
                        .AddProperty(this.GetDynamoDBUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                            .WithoutSetter()
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindByKeyAsync", method => method
                            .AddParameter(tPartitionKey, "partitionKey")
                            .AddParameter(tSortKey, "sortKey")
                            .AddOptionalCancellationTokenParameter()
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindByKeysAsync", method => method
                            .AddParameter($"IEnumerable<({tPartitionKey} Partition, {tSortKey} Sort)>", "keys")
                            .AddOptionalCancellationTokenParameter()
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindByIdAsync", method => method
                            .AddParameter($"({tPartitionKey} Partition, {tSortKey} Sort)", "id")
                            .AddOptionalCancellationTokenParameter()
                            .WithExpressionBody("FindByKeyAsync(id.Partition, id.Sort, cancellationToken)")
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", method => method
                            .AddParameter($"IEnumerable<({tPartitionKey} Partition, {tSortKey} Sort)>", "ids")
                            .AddOptionalCancellationTokenParameter()
                            .WithExpressionBody("FindByKeysAsync(ids, cancellationToken)")
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddOptionalCancellationTokenParameter()
                        )
                        ;
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            foreach (var model in Model)
            {
                var template = GetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, model.Id);

                template.CSharpFile.AfterBuild(file =>
                {
                    var primaryKeys = model.GetPrimaryKeyData();
                    if (primaryKeys == null)
                    {
                        throw new Exception("Could not get primary key data");
                    }

                    var @interface = file.Interfaces.Single();
                    @interface.Interfaces.Clear();

                    var genericArguments = new List<string>(3);

                    var tDomainGenericArgument = template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model);
                    if (model.GenericTypes.Any())
                    {
                        tDomainGenericArgument = $"{tDomainGenericArgument}<{string.Join(", ", model.GenericTypes)}>";
                    }

                    genericArguments.Add(tDomainGenericArgument);

                    var tPartitionKey = template.GetTypeName(primaryKeys.PartitionKeyAttribute.TypeReference);

                    genericArguments.Add(tPartitionKey);

                    if (primaryKeys.SortKeyAttribute != null)
                    {
                        genericArguments.Add(template.GetTypeName(primaryKeys.SortKeyAttribute.TypeReference));
                    }

                    @interface.ImplementsInterfaces($"{this.GetDynamoDBRepositoryInterfaceName()}<{string.Join(", ", genericArguments)}>");

                    var toRemove = @interface.Methods
                        .Where(x => x.Name is "FindByIdAsync" or "FindByIdsAsync")
                        .ToArray();

                    foreach (var method in toRemove)
                    {
                        @interface.Methods.Remove(method);
                    }
                }, 1000);
            }
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