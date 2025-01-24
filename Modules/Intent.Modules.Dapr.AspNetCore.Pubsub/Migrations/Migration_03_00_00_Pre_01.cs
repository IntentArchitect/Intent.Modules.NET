using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.Common.Templates;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.Dapr.AspNetCore.Pubsub.Api.MessageModelStereotypeExtensions;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Migrations
{
    public class Migration_03_00_00_Pre_01 : IModuleMigration
    {
        private readonly string MessageTypeId = "cbe970af-5bad-4d92-a3ed-a24b9fdaa23e";
        private readonly string TopicNamePropertyId = "a15b6899-7366-4fd7-b018-47a482e05432";
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_00_00_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Dapr.AspNetCore.Pubsub";
        [IntentFully]
        public string ModuleVersion => "3.0.0-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

            foreach (var package in packages)
            {
                var elements = GetMatchedElements(package.Classes, p => p.SpecializationTypeId == MessageTypeId);

                if (!elements.Any())
                {
                    continue;
                }

                foreach (var element in elements)
                {
                    foreach (var stereotype in element.Stereotypes.Where(x => x.DefinitionId == DaprSettings.DefinitionId))
                    {
                        var topicNameProperty = stereotype.Properties.FirstOrDefault(p => p.DefinitionId == TopicNamePropertyId);

                        if (topicNameProperty != null && string.IsNullOrEmpty(topicNameProperty.Value))
                        {
                            topicNameProperty.Value = $"{element.Name.RemoveSuffix("Event")}Event";
                        }
                    }
                }

                package.Save(true);
            }
        }

        public void Down()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

            foreach (var package in packages)
            {
                var elements = GetMatchedElements(package.Classes, p => p.SpecializationTypeId == MessageTypeId);

                if (!elements.Any())
                {
                    continue;
                }

                foreach (var element in elements)
                {
                    foreach (var stereotype in element.Stereotypes.Where(x => x.DefinitionId == DaprSettings.DefinitionId))
                    {
                        var topicNameProperty = stereotype.Properties.FirstOrDefault(p => p.DefinitionId == TopicNamePropertyId);

                        if (topicNameProperty != null && !string.IsNullOrEmpty(topicNameProperty.Value) && topicNameProperty.Value == $"{element.Name.RemoveSuffix("Event")}Event")
                        {
                            topicNameProperty.Value = string.Empty;
                        }
                    }
                }

                package.Save(true);
            }
        }

        private static IEnumerable<ElementPersistable> GetMatchedElements(IList<ElementPersistable> elements, Predicate<ElementPersistable> predicate)
        {
            foreach (var element in elements)
            {
                if (predicate(element))
                {
                    yield return element;
                }

                foreach (var result in GetMatchedElements(element.ChildElements, predicate))
                {
                    yield return result;
                }
            }
        }
    }
}