using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Interfaces.Comments
{
    /// <summary>
    /// My Service Comments
    /// </summary>
    public interface ICommentTestService : IDisposable
    {
        /// <summary>
        /// My Op Comments
        /// </summary>
        /// <param name="param1">Param 1 Comment</param>
        Task MyOp(string param1, CancellationToken cancellationToken = default);
    }
}