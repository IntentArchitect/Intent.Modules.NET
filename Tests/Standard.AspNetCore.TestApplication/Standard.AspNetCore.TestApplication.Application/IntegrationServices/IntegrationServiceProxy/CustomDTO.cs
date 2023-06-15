using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.IntegrationServiceProxy
{
    public class CustomDTO
    {
        public static CustomDTO Create(
            string referenceNumber)
        {
            return new CustomDTO
            {
                ReferenceNumber = referenceNumber,
            };
        }

        public string ReferenceNumber { get; set; }
    }
}