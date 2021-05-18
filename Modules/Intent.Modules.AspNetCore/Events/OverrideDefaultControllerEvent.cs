using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.AspNetCore.Events
{
    public class OverrideDefaultControllerEvent
    {
    }

    public class SecureTokenServiceHostedEvent
    {
        public SecureTokenServiceHostedEvent(string port)
        {
            Port = port;
        }
        public string Port { get; set; }
    }
}
