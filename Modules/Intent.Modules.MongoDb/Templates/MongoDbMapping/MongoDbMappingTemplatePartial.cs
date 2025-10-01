using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.MongoDb.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using FolderModel = Intent.Modules.Common.Types.Api.FolderModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

#nullable enable

namespace Intent.Modules.MongoDb.Templates.MongoDbMapping
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbMappingTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbMapping";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbMappingTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            var genericTypeParameters = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MongoDB.Bson.Serialization")
                .AddUsing("MongoDB.Bson.Serialization.IdGenerators")
                .AddUsing("MongoDB.Bson.Serialization.Serializers")
                .AddUsing("MongoDB.Bson")
                .AddClass($"{Model.Name}Mapping", @class =>
                {
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    @class.ImplementsInterface($"{GetTypeName(MongoMappingConfigurationInterface.MongoMappingConfigurationInterfaceTemplate.TemplateId)}<{Model.Name}{genericTypeParameters}>");

                    var collectionName = $"\"{Model.Name.Pluralize()}\"";
                    if (TryGetCollection(Model, out var relevantCollectionName))
                    {
                        collectionName = relevantCollectionName;
                    }
                    
                    @class.AddProperty("string", "CollectionName", p => p.WithoutSetter().Getter.WithExpressionImplementation(collectionName));
                    @class.AddMethod("void", "RegisterCollectionMap", registerCollectionMap =>
                    {
                        GetTypeName("Intent.Entities.DomainEntity", model.Id);
                        registerCollectionMap.AddIfStatement($"!BsonClassMap.IsClassMapRegistered(typeof({Model.Name}{genericTypeParameters}))", @if =>
                        {
                            @if.AddInvocationStatement($"BsonClassMap.RegisterClassMap<{Model.Name}{genericTypeParameters}>", invocation =>
                            {
                                invocation.AddLambdaBlock("mapping", block =>
                                {
                                    block.AddStatement("mapping.AutoMap();");
                                    if (ExecutionContext.GetSettings().GetMongoDBSettings().AlwaysIncludeDiscriminatorInDocuments())
                                    {
                                        block.AddStatement($"mapping.SetDiscriminator(nameof({Model.Name}{genericTypeParameters}));");
                                    }

                                    var pkAttribute = Model.GetPrimaryKeyAttribute();
                                    var pkType = GetPKType(pkAttribute);
                                    if (Model.ParentClass == null)
                                    {
                                        if (pkType == "string" && ExecutionContext.GetSettings().GetMongoDBSettings().PersistPrimaryKeyAsObjectId())
                                        {
                                            block.AddInvocationStatement("mapping.MapIdMember", s => s.AddArgument($"x => x.{pkAttribute.Name}")
                                            .AddInvocation("SetIdGenerator", si => si.AddArgument("StringObjectIdGenerator.Instance"))
                                            .AddInvocation("SetSerializer", si => si.AddArgument("new StringSerializer(MongoDB.Bson.BsonType.ObjectId)")));
                                        }
                                        else if (pkType == "Guid" && ExecutionContext.GetSettings().GetMongoDBSettings().PersistPrimaryKeyAsObjectId())
                                        {
                                            block.AddInvocationStatement("mapping.MapIdMember", s => s.AddArgument($"x => x.{pkAttribute.Name}")
                                            .AddInvocation("SetIdGenerator", si => si.AddArgument("CombGuidGenerator.Instance"))
                                            .AddInvocation("SetSerializer", si => si.AddArgument("new GuidSerializer(GuidRepresentation.Standard)")));
                                        }
                                        else
                                        {
                                            var dataSource = GetPKDataSource(pkAttribute);
                                            if (!string.IsNullOrEmpty(dataSource) && pkType == "string")
                                            {
                                                block.AddInvocationStatement("mapping.MapIdMember", s => s.AddArgument($"x => x.{pkAttribute.Name}")
                                                    .AddInvocation("SetIdGenerator", si => si.AddArgument("StringObjectIdGenerator.Instance")));
                                            }
                                            else if (!string.IsNullOrEmpty(dataSource) && pkType == "Guid")
                                            {
                                                block.AddInvocationStatement("mapping.MapIdMember", s => s.AddArgument($"x => x.{pkAttribute.Name}")
                                                    .AddInvocation("SetIdGenerator", si => si.AddArgument("CombGuidGenerator.Instance")));
                                            }
                                            else
                                            {
                                                block.AddInvocationStatement("mapping.MapIdMember", s => s.AddArgument($"x => x.{pkAttribute.Name}"));
                                            }
                                        }
                                    }
                                });
                            });
                        });
                    });
                });
        }

        private static bool TryGetCollection(ClassModel model, [NotNullWhen(true)]out string? relevantCollectionName)
        {
            relevantCollectionName = null;
            
            // Check the ClassModel itself
            if (model.TryGetCollection(out var classCollection))
            {
                relevantCollectionName = $"\"{classCollection.Name()}\"";
                return true;
            }
            
            // Check parent folders
            var currentElement = model.InternalElement.ParentElement;
            while (currentElement != null)
            {
                if (currentElement.SpecializationType == "Folder")
                {
                    var folderModel = new FolderModel(currentElement);
                    if (folderModel.TryGetCollection(out var folderCollection))
                    {
                        relevantCollectionName = $"\"{folderCollection.Name()}\"";
                        return true;
                    }
                }
                currentElement = currentElement.ParentElement;
            }
            
            return false;
        }

        internal string GetPKType(AttributeModel pkAttribute)
        {
            if (pkAttribute.Id != pkAttribute.Id)
            {
                return $"({GetTypeName(pkAttribute)} {pkAttribute.Name.ToPascalCase()},{GetTypeName(pkAttribute)} {pkAttribute.Name.ToPascalCase()})";
            }

            return GetTypeName(pkAttribute);
        }

        internal string GetPKDataSource(AttributeModel pkAttribute)
        {
            try
            {
                return pkAttribute.GetStereotype("64f6a994-4909-4a9d-a0a9-afc5adf2ef74").GetProperty("ce12cf69-e97f-401b-9b08-7e2c62171d4e").Value;
            }
            catch (Exception)
            {
                return string.Empty;
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