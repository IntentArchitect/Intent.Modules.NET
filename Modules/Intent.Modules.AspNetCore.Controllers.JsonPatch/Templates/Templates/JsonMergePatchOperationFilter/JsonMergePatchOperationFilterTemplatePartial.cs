using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchOperationFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JsonMergePatchOperationFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchOperationFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JsonMergePatchOperationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Mvc.ModelBinding")
                .AddUsing("Microsoft.OpenApi")
                .AddUsing("Morcatko.AspNetCore.JsonMergePatch")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddClass($"JsonMergePatchOperationFilter", @class =>
                {
                    @class.ImplementsInterface("IOperationFilter");

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter("OpenApiOperation", "operation");
                        method.AddParameter("OperationFilterContext", "context");
                        method.AddStatements(
                            $$"""
                                  var bodyParameters = context.ApiDescription.ParameterDescriptions.Where(p => p.Source == BindingSource.Body).ToList();

                                  foreach (var parameter in bodyParameters)
                                  {
                                      if (IsJsonMergePatchDocumentType(parameter.Type) && operation.RequestBody?.Content != null &&
                                          operation.RequestBody.Content.TryGetValue(JsonMergePatchDocument.ContentType, out var patchContent))
                                      {
                                          var schemaReference = (patchContent.Schema as OpenApiSchemaReference)?.Reference?.Id;
                                          if (!string.IsNullOrEmpty(schemaReference))
                                          {
                                              CleanUpSchemas(context, schemaReference);
                                          }

                                          var underlyingType = parameter.Type.GenericTypeArguments[0];
                                          var generatedSchema = GenerateSchema(context, underlyingType);
                                          RemovePatchIgnoredProperties(context, underlyingType, generatedSchema);
                                          if (generatedSchema is OpenApiSchema concreteGeneratedSchema)
                                          {
                                              patchContent.Schema = concreteGeneratedSchema;
                                          }
                                      }
                                      else if ((parameter.Type != null) && parameter.Type.IsGenericType && (parameter.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                                      {
                                          var jsonMergeType = parameter.Type.GenericTypeArguments[0];
                                          if (!IsJsonMergePatchDocumentType(jsonMergeType) || operation.RequestBody?.Content == null ||
                                              !operation.RequestBody.Content.TryGetValue(JsonMergePatchDocument.ContentType, out var enumerableContent))
                                          {
                                              continue;
                                          }

                                          string? itemsReference = null;
                                          if (enumerableContent.Schema is OpenApiSchemaReference schemaRef)
                                          {
                                              itemsReference = schemaRef.Reference?.Id;
                                          }
                                          else if (enumerableContent.Schema is OpenApiSchema concreteSchema && concreteSchema.Items is OpenApiSchemaReference itemsRef)
                                          {
                                              itemsReference = itemsRef.Reference?.Id;
                                          }

                                          if (!string.IsNullOrEmpty(itemsReference))
                                          {
                                              CleanUpSchemas(context, itemsReference);
                                          }

                                          var enumerableType = typeof(IEnumerable<>);
                                          var underlyingType = jsonMergeType.GenericTypeArguments[0];
                                          var genericEnumerableType = enumerableType.MakeGenericType(underlyingType);
                                          var generatedSchema = GenerateSchema(context, genericEnumerableType);
                                          RemovePatchIgnoredProperties(context, underlyingType, generatedSchema);
                                          if (generatedSchema is OpenApiSchema concreteEnumerableSchema)
                                          {
                                              enumerableContent.Schema = concreteEnumerableSchema;
                                          }
                                      }
                                  }
                                  """.ConvertToStatements());
                    });

                    @class.AddMethod("bool", "IsJsonMergePatchDocumentType", method =>
                    {
                        method.Private();
                        method.Static();
                        method.AddParameter("Type?", "t");
                        method.AddStatement("return (t != null) && t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>));");
                    });

                    @class.AddMethod("IOpenApiSchema", "GenerateSchema", method =>
                    {
                        method.Private();
                        method.Static();
                        method.AddParameter("OperationFilterContext", "context");
                        method.AddParameter("Type", "type");
                        method.AddStatement("return context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);");
                    });

                    @class.AddMethod("void", "CleanUpSchemas", method =>
                    {
                        method.Private();
                        method.Static();
                        method.AddParameter("OperationFilterContext", "context");
                        method.AddParameter("string", "jsonMergePatchSchemaId");
                        method.AddStatements(
                            """
                                var schemas = context.SchemaRepository.Schemas;
                                if (!schemas.TryGetValue(jsonMergePatchSchemaId, out var jsonMergePatchSchema))
                                {
                                    return;
                                }
                                
                                if (jsonMergePatchSchema is not OpenApiSchema concreteSchema)
                                {
                                    return;
                                }
                                
                                var contractResolverSchema = concreteSchema.Properties?["contractResolver"];
                                var operationsSchema = concreteSchema.Properties?["operations"];
                                schemas.Remove(jsonMergePatchSchemaId);
                                
                                if (contractResolverSchema?.AllOf?.Count > 0)
                                {
                                    var allOfFirstItem = contractResolverSchema.AllOf.First() as OpenApiSchemaReference;
                                    var contractResolverSchemaId = allOfFirstItem?.Reference?.Id;
                                    if (!string.IsNullOrEmpty(contractResolverSchemaId))
                                    {
                                        schemas.Remove(contractResolverSchemaId);
                                    }
                                }
                                
                                if (operationsSchema?.Items is OpenApiSchemaReference itemsRef)
                                {
                                    schemas.Remove(itemsRef.Reference.Id);
                                }
                                """.ConvertToStatements());
                    });

                    @class.AddMethod("void", "RemovePatchIgnoredProperties", method =>
                    {
                        method.Private();
                        method.Static();
                        method.AddParameter("OperationFilterContext", "context");
                        method.AddParameter("Type", "type");
                        method.AddParameter("IOpenApiSchema", "schema");
                        method.AddStatements(
                            """
                                if (schema is OpenApiSchemaReference schemaRef)
                                {
                                    var schemaId = schemaRef.Reference.Id;
                                    if (context.SchemaRepository.Schemas.TryGetValue(schemaId, out var referencedSchema))
                                    {
                                        if (referencedSchema is OpenApiSchema referencedConcreteSchema)
                                        {
                                            RemovePatchIgnoredPropertiesFromSchema(type, referencedConcreteSchema);
                                        }
                                    }
                                }
                                else if (schema is OpenApiSchema concreteSchema)
                                {
                                    RemovePatchIgnoredPropertiesFromSchema(type, concreteSchema);
                                }
                                """.ConvertToStatements());
                    });

                    @class.AddMethod("void", "RemovePatchIgnoredPropertiesFromSchema", method =>
                    {
                        method.Private();
                        method.Static();
                        method.AddParameter("Type", "type");
                        method.AddParameter("OpenApiSchema", "schema");
                        method.AddStatements(
                            """
                                if (schema?.Properties == null) return;
                                
                                var properties = type.GetProperties();
                                var propertiesToRemove = new List<string>();
                                
                                foreach (var property in properties)
                                {
                                    var hasIgnoreAttribute = property.GetCustomAttributes(true)
                                        .Any(a => a.GetType().Name == "PatchIgnoreAttribute");
                                    var hasPublicSetter = property.CanWrite && property.GetSetMethod(nonPublic: false) != null;
                                    if (!hasIgnoreAttribute && hasPublicSetter)
                                    {
                                        continue;
                                    }
                                
                                    var propertyName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                                    if (schema.Properties.ContainsKey(propertyName)) propertiesToRemove.Add(propertyName);
                                    else if (schema.Properties.ContainsKey(property.Name)) propertiesToRemove.Add(property.Name);
                                }
                                
                                foreach (var propertyName in propertiesToRemove)
                                {
                                    schema.Properties.Remove(propertyName);
                                }
                                """.ConvertToStatements());
                    });
                });
        }

        // This is a new Swashbuckle Filter so no need to support earlier versions, so if this somehow 
        // runs on an earlier OpenAPI version just don't generate.
        private bool IsMicrosoftOpenApi_2_4_1 => OutputTarget.GetMaxNetAppVersion().Major >= 8;

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   IsMicrosoftOpenApi_2_4_1 &&
                   ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")).Any();
        }

        public override void AfterTemplateRegistration()
        {
            if (!IsMicrosoftOpenApi_2_4_1)
            {
                return;
            }

            var templates =
                ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));

            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var cls = file.Classes.First();
                    var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(cls);
                    if (configureSwaggerOptionsBlock == null)
                    {
                        return;
                    }

                    configureSwaggerOptionsBlock.AddStatement($@"options.OperationFilter<{template.GetJsonMergePatchOperationFilterName()}>();");
                });
            }
        }

        private static CSharpLambdaBlock GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
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