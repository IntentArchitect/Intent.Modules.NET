using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Tests
{
    public class WindowsOnlyFact : FactAttribute
    {
        public WindowsOnlyFact()
        {
            if(Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                Skip = "Only supported on Windows";
            }
        }
    }
}
