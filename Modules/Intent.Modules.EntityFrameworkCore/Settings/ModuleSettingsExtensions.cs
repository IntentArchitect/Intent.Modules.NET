using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Settings
{

    public static class DatabaseSettingsExtensions
    {

        public static InheritanceStrategyOptions InheritanceStrategy(this DatabaseSettings groupSettings) => new InheritanceStrategyOptions(groupSettings.GetSetting("68f03894-248b-4569-bc76-52c67499bf7c")?.Value);

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