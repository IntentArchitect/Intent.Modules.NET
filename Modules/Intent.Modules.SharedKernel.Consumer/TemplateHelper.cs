using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.SharedKernel.Consumer
{
    public static class TemplateHelper
    {
        public static SharedKernel GetSharedKernel()
        {
            return new SharedKernel("7f0adaf7-dc33-4e21-bef2-085e5a45f2ab", "MicroKernelSample") ;
        }

    }

    public record SharedKernel(string ApplicationId, string ApplicationName);
}
