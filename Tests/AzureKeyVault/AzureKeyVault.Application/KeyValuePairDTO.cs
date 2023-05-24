using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureKeyVault.Application
{
    public class KeyValuePairDTO
    {
        public KeyValuePairDTO()
        {
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public static KeyValuePairDTO Create(string key, string value)
        {
            return new KeyValuePairDTO
            {
                Key = key,
                Value = value
            };
        }
    }
}