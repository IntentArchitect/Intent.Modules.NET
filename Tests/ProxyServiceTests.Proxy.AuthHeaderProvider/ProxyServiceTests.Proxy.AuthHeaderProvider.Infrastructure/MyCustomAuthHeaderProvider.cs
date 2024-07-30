using ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure.HttpClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Infrastructure
{
    internal class MyCustomAuthHeaderProvider : IAuthorizationHeaderProvider
    {
        public string? GetAuthorizationHeader()
        {
            throw new NotImplementedException();
        }
    }
}
