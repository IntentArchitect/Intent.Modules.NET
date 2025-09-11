using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBTableInitializer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBTableInitializerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBTableInitializer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBTableInitializerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Amazon.DynamoDBv2")
                .AddUsing("Amazon.DynamoDBv2.Model")
                .AddClass("DynamoDBTableInitializer", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "Initialize", method =>
                    {
                        method.Static().Async();
                        method.AddParameter("IAmazonDynamoDB", "client");

                        method.AddObjectInitializerBlock("var requests = new List<CreateTableRequest>", listBlock =>
                        {
                            var domainDesigner = ExecutionContext.MetadataManager.Domain(ExecutionContext.GetApplicationConfig().Id);
                            var aggregateRoots = domainDesigner.GetClassModels().Where(x => x.IsAggregateRoot() && IsDynamoDbEntity(x)).ToArray();
                            foreach (var aggregateRoot in aggregateRoots
                                         .OrderBy(x => x.Name)
                                         .ThenBy(x => x.InternalElement?.ParentElement?.Name)
                                         .ThenBy(x => x.InternalElement?.ParentElement?.ParentElement?.Name)
                                         .ThenBy(x => x.Id))
                            {
                                var definition = new TableDefinition(aggregateRoot);

                                listBlock.AddObjectInitializerBlock("new CreateTableRequest", requestBlock =>
                                {
                                    requestBlock.AddInitStatement("TableName", $"\"{definition.Name}\"");
                                    requestBlock.AddInitStatement("AttributeDefinitions", new CSharpObjectInitializerBlock("new List<AttributeDefinition>"),
                                        attributeListBlock =>
                                        {
                                            foreach (var attributeDefinition in definition.AttributeDefinitions)
                                            {
                                                ((CSharpObjectInitializerBlock)attributeListBlock).AddObjectInitializerBlock("new AttributeDefinition",
                                                    x =>
                                                    {
                                                        x.AddInitStatement("AttributeName", $"\"{attributeDefinition.Name}\"");
                                                        x.AddInitStatement("AttributeType", $"\"{attributeDefinition.Type}\"");
                                                    });
                                            }
                                        });
                                    requestBlock.AddInitStatement("KeySchema", new CSharpObjectInitializerBlock("new List<KeySchemaElement>"),
                                        attributeListBlock =>
                                        {
                                            foreach (var element in definition.KeySchemaElements)
                                            {
                                                ((CSharpObjectInitializerBlock)attributeListBlock).AddObjectInitializerBlock("new KeySchemaElement",
                                                    x =>
                                                    {
                                                        x.AddInitStatement("AttributeName", $"\"{element.Name}\"");
                                                        x.AddInitStatement("KeyType", $"\"{element.Type}\"");
                                                    });
                                            }
                                        });
                                    requestBlock.AddInitStatement(definition.Throughput.PropertyName, new CSharpObjectInitializerBlock($"new {definition.Throughput.TypeName}"),
                                        s =>
                                        {
                                            var initializerBlock = (CSharpObjectInitializerBlock)s;
                                            initializerBlock.AddInitStatement(definition.Throughput.ReadThroughputPropertyName, definition.Throughput.ReadThroughputValue.ToString("D"));
                                            initializerBlock.AddInitStatement(definition.Throughput.WriteThroughputPropertyName, definition.Throughput.WriteThroughputValue.ToString("D"));
                                        });
                                });
                            }

                            listBlock.WithSemicolon();
                        });

                        method.AddInvocationStatement("await Task.WhenAll", invocation =>
                        {
                            invocation.AddArgument(new CSharpInvocationStatement("requests.Select"), c =>
                            {
                                c.WithoutSemicolon();
                                c.AddLambdaBlock("async request", lambda =>
                                {
                                    lambda.AddStatement("// Don't create the table if it already exists");
                                    lambda.AddTryBlock(@try =>
                                    {
                                        @try.SeparatedFromPrevious(false);
                                        @try.AddStatement("var response = await client.DescribeTableAsync(request.TableName);");
                                        @try.AddIfStatement("response.Table != null", @if =>
                                        {
                                            @if.SeparatedFromPrevious(false);
                                            @if.AddStatement("return;");
                                        });
                                    });
                                    lambda.AddCatchBlock(@catch =>
                                    {
                                        @catch.WithExceptionType("ResourceNotFoundException");
                                        @catch.AddStatement("// NOP");
                                    });

                                    lambda.AddStatement("await client.CreateTableAsync(request);", s => s.SeparatedFromPrevious());
                                });
                            });
                        });
                    });
                });
        }

        private static bool IsDynamoDbEntity(ClassModel classModel)
        {
            var db = classModel.InternalElement.Package.AsDomainPackageModel()?.GetDocumentDatabase();
            if (db is null)
            {
                return false;
            }
            var provider = db.Provider();
            
            return provider == null || provider.Name == "DynamoDB";
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

        private class TableDefinition
        {
            public TableDefinition(ClassModel model)
            {
                var keys = model.GetPrimaryKeyData();
                if (keys == null)
                {
                    throw new Exception($"Could not determine keys for {model}");
                }

                if (!model.TryGetTableSettings(out var tableSettings))
                {
                    tableSettings = new TableSettings
                    {
                        ThroughputMode = TableThroughputMode.Provisioned,
                        ReadThroughput = 5,
                        WriteThroughput = 5
                    };
                }

                var tableSettingsName = !string.IsNullOrWhiteSpace(tableSettings.Name) ? tableSettings.Name : model.Name.ToPascalCase();
                Name = tableSettingsName.Pluralize().ToKebabCase();

                // Partition Key:
                {
                    var attributeName = keys.PartitionKeyAttribute.TryGetDynamoDBAttributeName(out var name)
                        ? name
                        : keys.PartitionKeyAttribute.Name.ToPascalCase();
                    AttributeDefinitions.Add(new AttributeDefinition
                    {
                        Name = attributeName,
                        Type = GetAttributeType(keys.PartitionKeyAttribute)
                    });

                    KeySchemaElements.Add(new KeySchemaElement
                    {
                        Name = attributeName,
                        Type = "HASH"
                    });
                }

                if (keys.SortKeyAttribute != null)
                {
                    var attributeName = keys.SortKeyAttribute.TryGetDynamoDBAttributeName(out var name)
                        ? name
                        : keys.SortKeyAttribute.Name.ToPascalCase();
                    AttributeDefinitions.Add(new AttributeDefinition
                    {
                        Name = attributeName,
                        Type = GetAttributeType(keys.SortKeyAttribute)
                    });

                    KeySchemaElements.Add(new KeySchemaElement
                    {
                        Name = attributeName,
                        Type = "RANGE"
                    });
                }

                Throughput = tableSettings.ThroughputMode switch
                {
                    TableThroughputMode.OnDemand => new Throughput
                    {
                        PropertyName = "OnDemandThroughput",
                        TypeName = "OnDemandThroughput",
                        ReadThroughputPropertyName = "MaxReadRequestUnits",
                        ReadThroughputValue = tableSettings.MaximumReadThroughput!.Value,
                        WriteThroughputPropertyName = "MaxWriteRequestUnits",
                        WriteThroughputValue = tableSettings.MaximumWriteThroughput!.Value
                    },
                    TableThroughputMode.Provisioned => new Throughput
                    {
                        PropertyName = "ProvisionedThroughput",
                        TypeName = "ProvisionedThroughput",
                        ReadThroughputPropertyName = "ReadCapacityUnits",
                        ReadThroughputValue = tableSettings.ReadThroughput!.Value,
                        WriteThroughputPropertyName = "WriteCapacityUnits",
                        WriteThroughputValue = tableSettings.WriteThroughput!.Value
                    },
                    _ => throw new InvalidOperationException($"Unknown throughput mode: {tableSettings.ThroughputMode}")
                };
            }

            public string Name { get; }
            public List<AttributeDefinition> AttributeDefinitions { get; } = [];
            public List<KeySchemaElement> KeySchemaElements { get; } = [];
            public Throughput Throughput { get; }
        }

        private static string GetAttributeType(AttributeModel model)
        {
            if (model.TypeReference.HasLongType() ||
                model.TypeReference.HasIntType())
            {
                return "N";
            }

            if (model.TypeReference.HasGuidType() ||
                model.TypeReference.HasStringType())
            {
                return "S";
            }

            throw new InvalidOperationException($"Unknown type: {model.TypeReference?.Element?.Name} ({model.TypeReference?.Element?.Id})");
        }

        private class AttributeDefinition
        {
            public required string Name { get; init; }
            public required string Type { get; init; }
        }

        private class KeySchemaElement
        {
            public required string Name { get; init; }
            public required string Type { get; init; }
        }

        private class Throughput
        {
            public required string PropertyName { get; init; }
            public required string TypeName { get; init; }
            public required string ReadThroughputPropertyName { get; init; }
            public required int ReadThroughputValue { get; init; }
            public required string WriteThroughputPropertyName { get; init; }
            public required int WriteThroughputValue { get; init; }
        }
    }
}