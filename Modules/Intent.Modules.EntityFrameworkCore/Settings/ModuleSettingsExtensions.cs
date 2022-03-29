using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static EntityFrameworkCoreSettings GetEntityFrameworkCoreSettings(this IApplicationSettingsProvider settings)
        {
            return new EntityFrameworkCoreSettings(settings.GetGroup("844a2692-4a23-4f53-8b29-0bd93067ebba"));
        }
    }

    public class EntityFrameworkCoreSettings
    {
        private readonly IGroupSettings _groupSettings;

        public EntityFrameworkCoreSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public InheritanceStrategyOptions InheritanceStrategy() => new InheritanceStrategyOptions(_groupSettings.GetSetting("68f03894-248b-4569-bc76-52c67499bf7c")?.Value);

        public class InheritanceStrategyOptions
        {
            public readonly string Value;

            public InheritanceStrategyOptions(string value)
            {
                Value = value;
            }

            public InheritanceStrategyOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "TPH" => InheritanceStrategyOptionsEnum.TablePerHierarchy,
                    "TPT" => InheritanceStrategyOptionsEnum.TablePerType,
                    "TPC" => InheritanceStrategyOptionsEnum.TablePerConcreteType,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsTablePerHierarchy()
            {
                return Value == "TPH";
            }

            public bool IsTablePerType()
            {
                return Value == "TPT";
            }

            public bool IsTablePerConcreteType()
            {
                return Value == "TPC";
            }
        }

        public enum InheritanceStrategyOptionsEnum
        {
            TablePerHierarchy,
            TablePerType,
            TablePerConcreteType,
        }
    }
}