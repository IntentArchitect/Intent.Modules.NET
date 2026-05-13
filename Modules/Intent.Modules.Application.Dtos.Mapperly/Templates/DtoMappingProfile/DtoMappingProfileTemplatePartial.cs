using Intent.Engine;
using Intent.IArchitect.Agent.Persistence;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Mapperly.Templates.DtoMappingProfile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DtoMappingProfileTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Mapperly.DtoMappingProfile";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoMappingProfileTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.RiokMapperly(outputTarget));

            /*These often are field types in the target dto. Not only value types*/
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.ValueObject);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Riok.Mapperly.Abstractions")
                .AddClass($"{Model.Name}Mapper", @class =>
                {
                    @class.Partial();
                    @class.AddAttribute("Mapper");

                    // Discover mapper dependencies INSIDE the class builder callback
                    var mapperDependencies = MappingHelper.DiscoverMapperDependencies(this, Model);

                    var entityTemplate = MappingHelper.GetEntityTemplate(this, Model);

                    var entityClassName = entityTemplate.ClassName;
                    var entityTypeName = GetTypeName(entityTemplate);

                    var dtoModelName = Model.Name;
                    var dtoTypeName = GetTypeName(TemplateRoles.Application.Contracts.Dto, Model);

                    // Analyze field mappings to determine what custom methods/attributes are needed
                    var mappingConfigurations = BuildMappingConfigurations();

                    // Identify unmapped source properties that need [MapperIgnoreSource] suppression
                    var unmappedSourceProperties = MappingHelper.GetUnmappedSourceProperties(this, Model);

                    // Generate [UseMapper] fields for each dependency
                    foreach (var dependency in mapperDependencies)
                    {
                        string fieldName = GetDependencyFieldName(dependency);
                        @class.AddField(this.GetTypeName(dependency), fieldName, field =>
                        {
                            field.PrivateReadOnly();
                            field.AddAttribute("UseMapper");
                        });
                    }

                    // Generate constructor if there are dependencies
                    if (mapperDependencies.Any())
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.Public();

                            foreach (var dependency in mapperDependencies)
                            {
                                var paramName = dependency.ClassName.ToCamelCase();
                                var fieldName = $"_{paramName}";

                                ctor.AddParameter(this.GetTypeName(dependency), paramName);
                                ctor.AddStatement($"{fieldName} = {paramName};");
                            }
                        });
                    }

                    // Generate mapping methods with attributes
                    @class.AddMethod(dtoTypeName, $"{entityClassName}To{dtoModelName}", method =>
                    {
                        method.Public().Partial();
                        method.AddParameter(entityTypeName, entityClassName.ToCamelCase()).WithoutMethodModifier();

                        // Add [MapperIgnoreSource] attributes for unmapped source properties
                        foreach (var unmappedProp in unmappedSourceProperties)
                        {
                            method.AddAttribute("MapperIgnoreSource", attribute =>
                            {
                                attribute.AddArgument(GetSafeMapperlyNameof(entityTypeName, unmappedProp));
                            });
                        }

                        // Add mapping configuration attributes
                        foreach (var config in mappingConfigurations)
                        {
                            if (config.UseCustomMethod)
                            {
                                method.AddAttribute("MapPropertyFromSource", attribute =>
                                {
                                    attribute.AddArgument(GetSafeMapperlyNameof(dtoTypeName, config.TargetPropertyName));
                                    attribute.AddArgument($"Use = nameof({config.CustomMethodName})");
                                });
                            }
                            else
                            {
                                method.AddAttribute("MapProperty", attribute =>
                                {
                                    attribute.AddArgument(GetSafeMapperlyNameof(entityTypeName, config.AttributePath));

                                    attribute.AddArgument(GetSafeMapperlyNameof(dtoTypeName, config.TargetPropertyName));
                                });
                            }
                        }
                    });

                    @class.AddMethod(UseType($"System.Collections.Generic.List<{dtoTypeName}>"), $"{entityClassName}To{dtoModelName}List", method =>
                    {
                        method.Public().Partial();
                        method.AddParameter($"IEnumerable<{entityTypeName}>", entityClassName.ToCamelCase().Pluralize()).WithoutMethodModifier();
                    });

                    // Generate custom mapping methods
                    foreach (var config in mappingConfigurations.Where(x => x.UseCustomMethod))
                    {
                        @class.AddMethod(GetTypeName(config.Field.TypeReference), config.CustomMethodName, method =>
                        {
                            method.Private();
                            method.AddParameter(entityTypeName, "source");
                            method.WithExpressionBody(BuildCustomMappingExpression(config, mapperDependencies));
                        });
                    }
                });
        }

        private static string GetDependencyFieldName(ICSharpFileBuilderTemplate dependency)
        {
            return $"_{dependency.ClassName.ToCamelCase()}";
        }

        /*See Mapperly docs on "full nameof".
         Nasty issues occur if you dont "full nameof" if the full path has propreties with the same name when C# nameof resolves
           */
        private static string GetSafeMapperlyNameof(string typeName, string memberPath)
        {
            var typeIsFullyQualified = typeName.Contains('.');
            var memberPathHopsOutsideClass = memberPath.Count(c => c == '.') >= 2;

            var useFullNameof = typeIsFullyQualified || memberPathHopsOutsideClass;

            return useFullNameof
                ? $"nameof(@{typeName}.{memberPath})"
                : $"nameof({typeName}.{memberPath})";
        }


        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .WithSingletonLifeTime()
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

        private sealed record MappingConfiguration(
            DTOFieldModel Field,
            string TargetPropertyName,
            string SourceExpression,
            string SourcePath,
            string AttributePath,
            bool UseCustomMethod,
            bool ShouldCast,
            string CustomMethodName);

        private IReadOnlyList<MappingConfiguration> BuildMappingConfigurations()
        {
            var configs = new List<MappingConfiguration>();

            foreach (var field in Model.Fields)
            {
                if (field.Mapping == null)
                {
                    continue;
                }

                var shouldCast = field.Mapping.Path.All(p => !string.IsNullOrEmpty(p.Id) &&
                                 GetTypeInfo(field.TypeReference).IsPrimitive &&
                                 field.Mapping.Element?.TypeReference != null &&
                                 GetFullyQualifiedTypeName(field.TypeReference) != GetFullyQualifiedTypeName(field.Mapping.Element.TypeReference));

                var (mappingExpression, attributePath) = MappingHelper.GetMappingExpression(this, field);

                var requiresExplicitMapping = mappingExpression != $"src.{field.Name}" || shouldCast;

                if (!requiresExplicitMapping && field.TypeReference.IsCollection)
                {
                    requiresExplicitMapping = field.TypeReference.Element.Name != field.Mapping.Element?.TypeReference?.Element.Name;
                }

                if (!requiresExplicitMapping)
                {
                    continue;
                }

                var targetPropertyName = field.Name.ToPascalCase();
                var sourcePath = mappingExpression.StartsWith("src.")
                    ? mappingExpression.Substring("src.".Length)
                    : mappingExpression;
                var useCustomMethod = shouldCast || RequiresCustomMethod(mappingExpression);

                // Check if Mapperly can handle nested property flattening by convention
                if (!useCustomMethod && attributePath.Contains(".") && !attributePath.Contains("("))
                {
                    var mapperlyConventionName = attributePath.Replace(".", "");
                    var actualTargetName = targetPropertyName;

                    if (mapperlyConventionName == actualTargetName)
                    {
                        continue;
                    }
                }

                var customMethodName = useCustomMethod ? $"Map{targetPropertyName}" : string.Empty;

                configs.Add(new MappingConfiguration(
                    Field: field,
                    TargetPropertyName: targetPropertyName,
                    SourceExpression: mappingExpression,
                    SourcePath: sourcePath,
                    AttributePath: attributePath,
                    UseCustomMethod: useCustomMethod,
                    ShouldCast: shouldCast,
                    CustomMethodName: customMethodName));
            }

            return configs;
        }

        private static bool RequiresCustomMethod(string mappingExpression)
        {
            var result = mappingExpression.Contains("(")
                   || mappingExpression.Contains("!")
                   || mappingExpression.Contains("?")
                   || mappingExpression.Contains(" ? ")
                   || mappingExpression.Contains(" : ");
            return result;
        }

        private string BuildCustomMappingExpression(MappingConfiguration config, IReadOnlyCollection<ICSharpFileBuilderTemplate> mapperDependencies)
        {
            var expression = config.SourceExpression.Replace("src.", "source.");
            var targetReference = config.Field.TypeReference;

            if (config.ShouldCast)
            {
                expression = $"({GetTypeName(config.Field.TypeReference)}){expression}";
            }
            if (config.Field.TypeReference.IsCollection)
            {
                var targetClassName = config.Field.TypeReference.Element.Name;
                var dependency = mapperDependencies.FirstOrDefault(x =>
                {
                    return x.TryGetModel<DTOModel>(out var dtoModel)
                           && dtoModel.Name == targetClassName;
                });
                if (dependency != null)
                {
                    var mapperFieldName = GetDependencyFieldName(dependency);
                    dependency.TryGetModel<DTOModel>(out var dtoModel); //TryGetModel guaranteed to pass since it was returned in the firstordefault above
                    var entityClassName = dtoModel.Mapping.Element.Name;
                    var modelClassName = dtoModel.Name;
                
                    expression = $"{expression}.Select({mapperFieldName}.{entityClassName}To{modelClassName})";

                }
            }
      
            return expression;
        }
    }
}
