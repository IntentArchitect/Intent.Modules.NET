using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Mapperly.Templates;
using Intent.Modules.Application.Dtos.Mapperly.Templates.DtoMappingProfile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Mapperly.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MapperlyRegistrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Dtos.Mapperly.MapperlyRegistrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // Find the Application DependencyInjection template
            var dependencyInjectionTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                TemplateRoles.Application.DependencyInjection);

            if (dependencyInjectionTemplate == null)
            {
                return;
            }

            // Find all Mapperly mapper templates
            var mapperTemplates = application
                .FindTemplateInstances<DtoMappingProfileTemplate>(DtoMappingProfileTemplate.TemplateId)
                .ToList();

            if (!mapperTemplates.Any())
            {
                return;
            }

            // Sort mappers by dependency depth (leaf mappers first)
            var sortedMappers = TopologicalSort(mapperTemplates);

            // Add registrations to DependencyInjection
            dependencyInjectionTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod("AddApplication");

                if (method == null)
                {
                    return;
                }

                // Find the return statement to insert before it
                var returnStatement = method.FindStatement(s => s.GetText("").Trim().StartsWith("return"));

                foreach (var mapper in sortedMappers)
                {
                    var registrationStatement = $"services.AddSingleton<{mapper.ClassName}>();";

                    if (returnStatement != null)
                    {
                        returnStatement.InsertAbove(registrationStatement);
                    }
                    else
                    {
                        method.AddStatement(registrationStatement);
                    }
                }
            });
        }

        /// <summary>
        /// Performs topological sort to ensure mappers are registered in dependency order.
        /// Leaf mappers (with no dependencies) come first, parent mappers come after.
        /// </summary>
        private List<DtoMappingProfileTemplate> TopologicalSort(List<DtoMappingProfileTemplate> mappers)
        {
            var sorted = new List<DtoMappingProfileTemplate>();
            var visited = new HashSet<string>();
            var processed = new HashSet<string>();

            foreach (var mapper in mappers)
            {
                Visit(mapper, mappers, visited, processed, sorted);
            }

            return sorted;
        }

        private void Visit(
            DtoMappingProfileTemplate mapper,
            List<DtoMappingProfileTemplate> allMappers,
            HashSet<string> visited,
            HashSet<string> processed,
            List<DtoMappingProfileTemplate> sorted)
        {
            if (processed.Contains(mapper.Id))
            {
                return;
            }

            if (visited.Contains(mapper.Id))
            {
                // Circular dependency detected - just skip for now
                return;
            }

            visited.Add(mapper.Id);

            // Get dependencies for this mapper
            var dependencies = MappingHelper.DiscoverMapperDependencies(mapper, mapper.Model);

            // Visit dependencies first
            foreach (var dependency in dependencies)
            {
                var dependencyMapper = allMappers.FirstOrDefault(m => m.Id == dependency.Id);
                if (dependencyMapper != null)
                {
                    Visit(dependencyMapper, allMappers, visited, processed, sorted);
                }
            }

            processed.Add(mapper.Id);
            sorted.Add(mapper);
        }
    }
}
