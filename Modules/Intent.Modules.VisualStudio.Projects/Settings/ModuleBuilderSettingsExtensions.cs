using System;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Settings;

// Generated initially but left like this in order to access these settings if available 
public static class ModuleBuilderSettingsExtensions
{
    public static DependencyVersionOverwriteBehaviorOptions DependencyVersionOverwriteBehavior(this IApplicationSettingsProvider groupSettings) => new DependencyVersionOverwriteBehaviorOptions(groupSettings.GetSetting("b2c4252b-cfae-43c5-9682-803aa0b84c87", "9c8e6982-e036-4d35-bab1-9bb02382d7c3")?.Value);

    public class DependencyVersionOverwriteBehaviorOptions
    {
        public readonly string Value;

        public DependencyVersionOverwriteBehaviorOptions(string value)
        {
            Value = value;
        }

        public DependencyVersionOverwriteBehaviorOption AsEnum()
        {
            return Value switch
            {
                "always" => DependencyVersionOverwriteBehaviorOption.Always,
                "if-newer" => DependencyVersionOverwriteBehaviorOption.IfNewer,
                "never" => DependencyVersionOverwriteBehaviorOption.Never,
                _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
            };
        }

        public bool IsAlways()
        {
            return Value == "always";
        }

        public bool IsIfNewer()
        {
            return Value == "if-newer";
        }

        public bool IsNever()
        {
            return Value == "never";
        }
    }
}
    
public enum DependencyVersionOverwriteBehaviorOption
{
    Always,
    IfNewer,
    Never,
}