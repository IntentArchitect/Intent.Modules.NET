using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.MongoDb.Templates.MongoDbMapping;
using Intent.Modules.MongoDb.Templates.MongoDbRepository;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoConfigurationExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoConfigurationExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoConfigurationExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoConfigurationExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("MongoDB.Driver")
                .AddUsing("System.Reflection")
                .AddUsing("System.Linq")
                .AddUsing("System")
                .AddClass($"MongoConfigurationExtensions", @class =>
                {
                    @class.Static().Internal();

                    @class.AddMethod("IServiceCollection", "AddMongoCollection<T>", addMongoCollection =>
                    {
                        addMongoCollection.Static();
                        addMongoCollection.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        addMongoCollection.AddParameter("IMongoMappingConfiguration<T>", "mongoConfiguration");

                        addMongoCollection.AddStatement("mongoConfiguration.RegisterCollectionMap();");

                        addMongoCollection.AddStatements($@"services.AddSingleton(sp =>
                        {{
                            var database = sp.GetRequiredService<IMongoDatabase>();
                            return database.GetCollection<T>(mongoConfiguration.CollectionName);
                        }});".ConvertToStatements());

                        addMongoCollection.AddReturn("services");
                    });

                    var domainEntities = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Entities.DomainEntity");
                    var toExclude = new List<string>();
                    foreach (var model in domainEntities)
                    {

                        if (model.CSharpFile.Classes.First().IsAbstract && model.CSharpFile.Classes.First().GenericParameters.Count > 0)
                        {
                            GetTypeName(model);

                            toExclude.Add(model.ClassName);
                            @class.AddMethod("void", $"Register{model.ClassName}Mappings", registerMongoCollections =>
                            {
                                registerMongoCollections.Static();
                                registerMongoCollections.AddParameter("Assembly", "assembly");

                                registerMongoCollections.AddStatement($"var baseType = typeof({model.ClassName}<>);");

                                registerMongoCollections.AddStatements(@$"var derivedTypes = assembly.GetTypes()
                                    .Where(t => t.BaseType != null &&
                                                t.BaseType.IsGenericType &&
                                                t.BaseType.GetGenericTypeDefinition() == baseType);".ConvertToStatements());

                                registerMongoCollections.AddForEachStatement("derivedType", "derivedTypes", @foreach =>
                                {
                                    @foreach.AddStatement("var genericArg = derivedType.BaseType!.GetGenericArguments()[0];");
                                    @foreach.AddStatement($"var closedMappingType = typeof({model.ClassName}Mapping<>).MakeGenericType(genericArg);");
                                    @foreach.AddStatement("var mappingInstance = Activator.CreateInstance(closedMappingType);");
                                    @foreach.AddStatement($"var registerMethod = closedMappingType.GetMethod(nameof({model.ClassName}Mapping<object>.RegisterCollectionMap));");
                                    
                                    @foreach.AddStatement("registerMethod?.Invoke(mappingInstance, null);");
                                });
                            });
                        }
                    }

                    @class.AddMethod("IServiceCollection", "RegisterMongoCollections", registerMongoCollections =>
                    {
                        registerMongoCollections.Static();
                        registerMongoCollections.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        registerMongoCollections.AddParameter("Assembly", "assembly");


                        // Foreach model mapping
                        foreach (var model in ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(MongoDbMappingTemplate.TemplateId))
                        {
                            GetTypeName(model);

                            if (!toExclude.Contains(model.ClassName.Replace("Mapping", "")))
                            {
                                registerMongoCollections.AddStatement($"services.AddMongoCollection(new {model.ClassName}());");
                            }
                            else
                            {
                                registerMongoCollections.AddStatement($"Register{model.ClassName}s(assembly);");
                            }
                        }

                        registerMongoCollections.AddReturn("services");
                    });


                });
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