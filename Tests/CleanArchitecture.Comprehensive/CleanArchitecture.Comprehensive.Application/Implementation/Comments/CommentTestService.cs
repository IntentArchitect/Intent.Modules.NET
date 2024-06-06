using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Application.Interfaces.Comments;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Implementation.Comments
{
    /// <summary>
    /// My Service Comments
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class CommentTestService : ICommentTestService
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommentTestService()
        {
        }

        /// <summary>
        /// My Op Comments
        /// </summary>
        /// <param name="param1">Param 1 Comment</param>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task MyOp(string param1, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        public void Dispose()
        {
        }
    }
}