using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Riok.Mapperly.Abstractions")
                .AddClass($"{Model.Name}Mapper", @class =>
                {
                    @class.Partial();
                    @class.AddAttribute("Mapper");

                    // Discover mapper dependencies INSIDE the class builder callback
                    var mapperDependencies = MappingHelper.DiscoverMapperDependencies(this, Model);

                    // DEBUG: Log what we found
                    Logging.Log.Debug($"[Mapperly] {Model.Name}Mapper: Found {mapperDependencies.Count} dependencies: {string.Join(", ", mapperDependencies.Select(d => d.ClassName))}");

                    var entityTemplate = MappingHelper.GetEntityTemplate(this, Model);
                    string dtoModelName = GetTypeName(TemplateRoles.Application.Contracts.Dto, Model);
                    string entityTypeName = this.GetTypeName(entityTemplate);

                    // Analyze field mappings to determine what custom methods/attributes are needed
                    var mappingConfigurations = BuildMappingConfigurations(entityTypeName);
                    Logging.Log.Debug($"[Mapperly FieldMappings] {Model.Name}Mapper: Found {mappingConfigurations.Count} fields requiring explicit mapping");

                    // Identify unmapped source properties that need [MapperIgnoreSource] suppression
                    var unmappedSourceProperties = MappingHelper.GetUnmappedSourceProperties(this, Model);
                    Logging.Log.Debug($"[Mapperly UnmappedSource] {Model.Name}Mapper: Found {unmappedSourceProperties.Count} unmapped source properties to suppress");

                    // Generate [UseMapper] fields for each dependency
                    foreach (var dependency in mapperDependencies)
                    {
                        var fieldName = $"_{dependency.ClassName.ToCamelCase()}";
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
                    @class.AddMethod(dtoModelName, $"{entityTypeName}To{dtoModelName}", method =>
                    {
                        method.Public().Partial();
                        method.AddParameter(entityTypeName, entityTypeName.ToCamelCase()).WithoutMethodModifier();

                        // Add [MapperIgnoreSource] attributes for unmapped source properties
                        foreach (var unmappedProp in unmappedSourceProperties)
                        {
                            method.AddAttribute("MapperIgnoreSource", attribute =>
                            {
                                attribute.AddArgument($"nameof({entityTypeName}.{unmappedProp})");
                            });
                        }

                        // Add mapping configuration attributes
                        foreach (var config in mappingConfigurations)
                        {
                            if (config.UseCustomMethod)
                            {
                                method.AddAttribute("MapPropertyFromSource", attribute =>
                                {
                                    attribute.AddArgument($"nameof({dtoModelName}.{config.TargetPropertyName})");
                                    attribute.AddArgument($"Use = nameof({config.CustomMethodName})");
                                });
                            }
                            else
                            {
                                method.AddAttribute("MapProperty", attribute =>
                                {
                                    attribute.AddArgument($"nameof({entityTypeName}.{config.SourcePath})");
                                    attribute.AddArgument($"nameof({dtoModelName}.{config.TargetPropertyName})");
                                });
                            }
                        }
                    });

                    @class.AddMethod($"List<{dtoModelName}>", $"{entityTypeName}To{dtoModelName}List", method =>
                    {
                        method.Public().Partial();
                        method.AddParameter($"List<{entityTypeName}>", entityTypeName.ToCamelCase().Pluralize()).WithoutMethodModifier();
                    });

                    // Generate custom mapping methods
                    foreach (var config in mappingConfigurations.Where(x => x.UseCustomMethod))
                    {
                        @class.AddMethod(GetTypeName(config.Field.TypeReference), config.CustomMethodName, method =>
                        {
                            method.Private();
                            method.AddParameter(entityTypeName, "source");
                            method.WithExpressionBody(BuildCustomMappingExpression(config));
                        });
                    }
                });
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
            bool UseCustomMethod,
            bool ShouldCast,
            string CustomMethodName);

        private IReadOnlyList<MappingConfiguration> BuildMappingConfigurations(string entityTypeName)
        {
            var configs = new List<MappingConfiguration>();

            Logging.Log.Debug($"[Mapperly FieldMappings] {Model.Name}: Analyzing {Model.Fields.Count} fields");

            foreach (var field in Model.Fields)
            {
                // DEBUG: Log each field
                var hasMappingStr = field.Mapping != null ? "YES" : "NO";
                Logging.Log.Debug($"[Mapperly FieldMappings]   Field: {field.Name}, HasMapping: {hasMappingStr}");

                if (field.Mapping == null)
                {
                    continue;
                }

                // DEBUG: Log mapping path
                var pathStr = string.Join(" -> ", field.Mapping.Path.Select(p => $"{p.Name}({p.Specialization})"));
                Logging.Log.Debug($"[Mapperly FieldMappings]     Mapping Path: {pathStr}");

                var shouldCast = field.Mapping.Path.All(p => !string.IsNullOrEmpty(p.Id) &&
                                 GetTypeInfo(field.TypeReference).IsPrimitive &&
                                 field.Mapping.Element?.TypeReference != null &&
                                 GetFullyQualifiedTypeName(field.TypeReference) != GetFullyQualifiedTypeName(field.Mapping.Element.TypeReference));

                var (mappingExpression, _) = MappingHelper.GetMappingExpression(this, field);
                Logging.Log.Debug($"[Mapperly FieldMappings]     Mapping Expression: {mappingExpression}");

                var requiresExplicitMapping = mappingExpression != $"src.{field.Name}" || shouldCast;

                if (!requiresExplicitMapping && field.TypeReference.IsCollection)
                {
                    requiresExplicitMapping = field.TypeReference.Element.Name != field.Mapping.Element?.TypeReference?.Element.Name;
                }

                if (!requiresExplicitMapping)
                {
                    Logging.Log.Debug($"[Mapperly FieldMappings]     SKIPPED: Convention mapping will work");
                    continue;
                }

                var targetPropertyName = field.Name.ToPascalCase();
                var sourcePath = mappingExpression.StartsWith("src.")
                    ? mappingExpression.Substring("src.".Length)
                    : mappingExpression;
                var useCustomMethod = shouldCast || RequiresCustomMethod(mappingExpression);

                // Check if Mapperly can handle nested property flattening by convention
                if (!useCustomMethod && sourcePath.Contains(".") && !sourcePath.Contains("("))
                {
                    var mapperlyConventionName = sourcePath.Replace(".", "");
                    var actualTargetName = targetPropertyName;

                    if (mapperlyConventionName == actualTargetName)
                    {
                        Logging.Log.Debug($"[Mapperly FieldMappings]     SKIPPED: {sourcePath} → {actualTargetName} matches Mapperly flattening convention");
                        continue;
                    }
                    else
                    {
                        Logging.Log.Debug($"[Mapperly FieldMappings]     EXPLICIT: {sourcePath} convention={mapperlyConventionName} but field={actualTargetName}");
                    }
                }

                var customMethodName = useCustomMethod ? $"Map{targetPropertyName}" : string.Empty;

                Logging.Log.Debug($"[Mapperly FieldMappings]     EXPLICIT MAPPING NEEDED: UseCustomMethod={useCustomMethod}, SourcePath={sourcePath}");

                configs.Add(new MappingConfiguration(
                    Field: field,
                    TargetPropertyName: targetPropertyName,
                    SourceExpression: mappingExpression,
                    SourcePath: sourcePath,
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
                   || mappingExpression.Contains(" ? ")
                   || mappingExpression.Contains(" : ");
            return result;
        }

        private string BuildCustomMappingExpression(MappingConfiguration config)
        {
            var expression = config.SourceExpression.Replace("src.", "source.");
            if (config.ShouldCast)
            {
                expression = $"({GetTypeName(config.Field.TypeReference)}){expression}";
            }

            return expression;
        }
    }
}