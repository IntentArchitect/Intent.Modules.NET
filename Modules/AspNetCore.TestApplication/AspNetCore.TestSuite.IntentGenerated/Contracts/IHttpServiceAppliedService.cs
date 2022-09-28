using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AspNetCore.TestSuite.IntentGenerated.Contracts
{

    public interface IHttpServiceAppliedService : IDisposable
    {

        Task<string> GetValue();

        Task PostValue(string value);

        Task NonAppliedOperation();

    }
}