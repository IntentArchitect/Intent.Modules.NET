using System.Collections.Generic;
using Namespace;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates
{
    public class LaunchSettingsTests
    {
        [Fact]
        public void ItShouldWork()
        {
            var ls = new LaunchSettings();
            ls.Profiles = new Dictionary<string, Profile>
            {
                ["mine"] = new()
                {
                    CommandName = CommandName.Project
                }
            };


            var serialized = ls.ToJson();
        }
    }
}
