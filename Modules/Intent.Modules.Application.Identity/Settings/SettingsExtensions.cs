using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Application.Identity.Settings.IdentitySettings;

namespace Intent.Modules.Application.Identity.Settings
{
    public static class SettingsExtensions
    {
        public static string ToCSharpType(this UserIdTypeOptions option)
        {
            return option.Value switch
            {
                "guid" => "System.Guid",
                "long" => "long",
                "int" => "int",
                _ => "string"
            };
        }
    }
}
