using System;
using System.Collections.Generic;
using AzureKeyVault.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureKeyVault.Application.GetKeyValues
{
    public class GetKeyValues : IRequest<KeyValuesDTO>, IQuery
    {
    }
}