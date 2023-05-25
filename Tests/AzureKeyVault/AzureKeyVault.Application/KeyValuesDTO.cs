using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureKeyVault.Application
{
    public class KeyValuesDTO
    {
        public KeyValuesDTO()
        {
            Keys = null!;
        }

        public List<KeyValuePairDTO> Keys { get; set; }

        public static KeyValuesDTO Create(List<KeyValuePairDTO> keys)
        {
            return new KeyValuesDTO
            {
                Keys = keys
            };
        }
    }
}